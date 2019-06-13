using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnUtils.Application.Modules;
using OnUtils.Data;

namespace OnWeb.Core.ModuleExtensions.CustomFields
{
    using DB;
    using Modules;
    using Modules.Extensions;
    using Scheme;

#pragma warning disable CS1591 // todo внести комментарии.
    public class ExtensionCustomsFieldsBase : ModuleExtension, IUnitOfWorkAccessor<Context>
    {
        public class CacheCollection : IReadOnlyDictionary<SchemeItem, DefaultScheme>
        {
            private ExtensionCustomsFieldsBase _extension = null;
            private IList<CustomFieldsScheme> _schemes = null;
            private Dictionary<SchemeItem, DefaultScheme> _dictionary = new Dictionary<SchemeItem, DefaultScheme>();

            internal CacheCollection(ExtensionCustomsFieldsBase extension)
            {
                using (var db = extension.CreateUnitOfWork())
                {
                    this._extension = extension;
                    this._schemes = db.CustomFieldsSchemes.Where(x => x.IdModule == _extension.Module.ID && x.IdScheme > 0).ToList();
                }
            }

            private DefaultScheme CreateDefaultScheme()
            {
                var def = new DefaultScheme();

                foreach (var scheme in _schemes)
                {
                    def.Schemes[(uint)scheme.IdScheme] = new Scheme.Scheme(null, def)
                    {
                        IdScheme = (uint)scheme.IdScheme,
                        NameScheme = scheme.NameScheme
                    };
                }

                return def;
            }

            #region IReadOnlyDictionary<Scheme.SchemeItem, Scheme.DefaultScheme>
            public IEnumerable<SchemeItem> Keys
            {
                get => _dictionary.Keys;
            }

            public IEnumerable<DefaultScheme> Values
            {
                get => _dictionary.Values;
            }

            public int Count
            {
                get => _dictionary.Count;
            }

            public bool ContainsKey(SchemeItem key)
            {
                return true;
            }

            public bool TryGetValue(SchemeItem key, out DefaultScheme value)
            {
                value = this[key];
                return true;
            }

            public IEnumerator<KeyValuePair<SchemeItem, DefaultScheme>> GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
            #endregion

            public DefaultScheme this[SchemeItem key]
            {
                get
                {
                    if (!_dictionary.ContainsKey(key)) _dictionary[key] = CreateDefaultScheme();
                    return _dictionary[key];
                }
            }
        }

        public static readonly Guid PERM_EXTFIELDS_ALLOWMANAGE = "PERM_EXTFIELDS_ALLOWMANAGE".GenerateGuid();

        private object SyncRoot = new object();
        private CacheCollection _cache = null;

        public ExtensionCustomsFieldsBase(ModuleCore moduleObject)
            : base(moduleObject)
        {
            moduleObject.RegisterPermission(PERM_EXTFIELDS_ALLOWMANAGE, "Настройка схемы полей");

            using (var db = this.CreateUnitOfWork())
                db.DataContext.ExecuteQuery($"UPDATE CustomFieldsSchemeData SET IdItemType='{ModuleCore.CategoryType}' WHERE IdModule='{Module.ID}' AND IdItemType=0");

            Task.Delay(60000).ContinueWith(t => TimerCallback());
        }

        #region Property
        protected CacheCollection Cache
        {
            get
            {
                lock (SyncRoot)
                {
                    if (_cache == null) CreateCache();
                    return _cache;
                }
            }
        }

        protected bool AllowSchemeManage
        {
            get => Module.CheckPermission(PERM_EXTFIELDS_ALLOWMANAGE) == CheckPermissionResult.Allowed;
        }
        #endregion

        #region Cache
        private void CreateCache()
        {
            var measure = new MeasureTime();
            Exception exception = null;

            try
            {
                lock (SyncRoot)
                {
                    _cache = null;

                    var IdModule = this.Module.ID;

                    using (var db = this.CreateUnitOfWork())
                    {
                        var q_items = (from ip in db.ItemParent
                                       where ip.IdModule == IdModule
                                       group ip by new { ip.IdItem, ip.IdItemType } into grp
                                       select grp.Key);

                        var q_fields = db.CustomFieldsFields.Where(x => x.IdModule == IdModule && x.Block == 0).Include(x => x.data).ToList();

                        var items = (from p in db.ItemParent
                                     join ip in q_items on new { p.IdItem, p.IdItemType } equals new { ip.IdItem, ip.IdItemType }
                                     join sd in db.CustomFieldsSchemeDatas on new { p.IdModule, p.IdItemType, IdItem = p.IdParentItem } equals new { sd.IdModule, sd.IdItemType, IdItem = sd.IdSchemeItem }
                                     join sch in db.CustomFieldsSchemes on sd.IdScheme equals sch.IdScheme into joinedScheme
                                     from sch in joinedScheme.DefaultIfEmpty()
                                     join fi in db.CustomFieldsFields on sd.IdField equals fi.IdField
                                     where fi.Block == 0
                                     orderby p.IdItemType ascending, p.IdItem ascending, p.IdLevel ascending, sd.IdScheme ascending, sd.Order ascending
                                     select new { ItemParent = p, SchemeData = sd, Scheme = sch, Field = fi });

                        var fields = new Dictionary<int, DB.CustomFieldsField>();
                        var patrs = new CacheCollection(this);
                        foreach (var ppp in items)
                        {
                            if (ppp.SchemeData.IdScheme > 0 && ppp.Scheme == null) continue;

                            var schemeItem = new Scheme.SchemeItem(ppp.ItemParent.IdItem, ppp.ItemParent.IdItemType);
                            var key = string.Format("{0}_{1}", ppp.ItemParent.IdItemType, ppp.ItemParent.IdItem);
                            if (!fields.ContainsKey(ppp.Field.IdField)) fields.Add(ppp.Field.IdField, ppp.Field);

                            var patr = patrs[schemeItem];

                            var field = fields[ppp.Field.IdField];

                            /*
                             * Хак на время запуска - схемы 0 может и не существовать. На всякий случай собираем все поля для схемы Default из остальных схем.
                             * */
                            patr[field.IdField] = field;
                            if (field.data != null) field.data = field.FieldType.CreateValuesCollection(field, field.data);

                            if (ppp.SchemeData.IdScheme == 0) patr[field.IdField] = field;
                            else
                            {
                                if (!patr.Schemes.ContainsKey((uint)ppp.SchemeData.IdScheme))
                                {
                                    var sch = new Scheme.Scheme(null, patr)
                                    {
                                        IdScheme = (uint)ppp.Scheme.IdScheme,
                                        NameScheme = ppp.Scheme.NameScheme
                                    };
                                    patr.Schemes.Add((uint)ppp.SchemeData.IdScheme, sch);
                                }
                                (patr.Schemes[(uint)ppp.SchemeData.IdScheme] as Scheme.Scheme)[field.IdField] = field;
                            }
                        }

                        _cache = patrs;
                    }
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                if (exception == null && measure.Calculate(false).TotalMilliseconds >= 50)
                    Debug.WriteLineNoLog("CreateCache: {0} items, {1}ms", _cache != null ? _cache.Count : 0, measure);
                else if (exception != null)
                    Debug.WriteLine("CreateCacheFailed: {0}", exception.GetLowLevelException().Message);
            }
        }

        internal void ClearCache(bool norecursive = false)
        {
            lock (SyncRoot)
            {
                _cache = null;

                if (!norecursive)
                    foreach (var extension in this.Module.GetExtensions())
                        if (extension is ExtensionCustomsFieldsBase && !this.Equals(extension))
                            (extension as ExtensionCustomsFieldsBase).ClearCache(true);

                Proxy.ProxyHelper.ClearCache();
            }

        }

        public void UpdateCache()
        {
            lock (SyncRoot)
            {
                ClearCache();
                CreateCache();
            }
        }
        #endregion

        #region Deffered
        #region Clear links
        private class TimerData
        {
            public DateTime DateCreated { get; } = DateTime.Now;

            public Dictionary<Items.ItemBase, Type> Objects = new Dictionary<Items.ItemBase, Type>();
        }

        private ConcurrentQueue<TimerData> _linksListsQueue = new ConcurrentQueue<TimerData>();

        private TimerData TimerClearDeffered
        {
            get
            {
                TimerData data;
                if (!_linksListsQueue.IsEmpty)
                {
                    var last = _linksListsQueue.Last();
                    if ((DateTime.Now - last.DateCreated).TotalMinutes >= 1)
                    {
                        data = new TimerData();
                        _linksListsQueue.Enqueue(data);
                        return data;
                    }
                    else return last;
                }
                else
                {
                    data = new TimerData();
                    _linksListsQueue.Enqueue(data);
                    return data;
                }
            }
        }

        private void TimerCallback()
        {
            try
            {
                TimerData dataPeek;
                TimerData dataDequeued;
                while (!_linksListsQueue.IsEmpty)
                {
                    if (_linksListsQueue.TryPeek(out dataPeek))
                    {
                        if ((DateTime.Now - dataPeek.DateCreated).TotalMinutes >= 1)
                        {
                            if (_linksListsQueue.TryDequeue(out dataDequeued) && object.ReferenceEquals(dataDequeued, dataPeek))
                            {
                                if (dataDequeued.Objects.Count > 0)
                                {
                                    var objectsGroupedByType = dataDequeued.Objects.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.ToList());
                                    foreach (var pair in objectsGroupedByType)
                                    {
                                        var objType = pair.Key;
                                        ConcurrentBag<Items.ItemBase> list = null;
                                        if (_defferedObjects.TryGetValue(objType, out list))
                                        {
                                            Items.ItemBase item;
                                            foreach (var obj in pair.Value)
                                            {
                                                list.TryTake(out item);
                                            }
                                        }
                                    }
                                    dataDequeued.Objects.Clear();
                                }
                            }
                        }
                        else break;
                    }
                    else break;
                }
            }
            finally
            {
                Task.Delay(60000).ContinueWith(t => TimerCallback());
            }
        }
        #endregion

        private ConcurrentDictionary<Type, ConcurrentBag<Items.ItemBase>> _defferedObjects = new ConcurrentDictionary<Type, ConcurrentBag<Items.ItemBase>>();

        public void RegisterToQuery<TItem>(TItem obj) where TItem : Items.ItemBase
        {
            var objType = obj.GetType();
            var list = _defferedObjects.GetOrAdd(objType, k => new ConcurrentBag<Items.ItemBase>());

            if (!list.Contains(obj)) list.Add(obj);

            TimerClearDeffered.Objects[obj] = objType;
        }

        internal void CheckDeffered(Type type = null)
        {
            foreach (var pair in _defferedObjects)
            {
                if (type != null & pair.Key != type) continue;

                var items = pair.Value.ToList();

                int start = 0;
                List<Items.ItemBase> subItems = null;

                do
                {
                    subItems = items.Skip(start).Take(500).ToList();
                    start += 500;
                    if (subItems.Count() > 0)
                    {
                        var fieldsForItems = GetItemsFields(subItems);
                        if (fieldsForItems != null)
                        {
                            foreach (var p in fieldsForItems)
                            {
                                p.Key._fields = p.Value;
                            }

                            foreach (var p in subItems)
                            {
                                p._fields = p.Fields;
                            }
                        }
                    }
                    else break;
                } while (subItems.Count() > 0);

                Items.ItemBase someItem;
                while (!pair.Value.IsEmpty) pair.Value.TryTake(out someItem);
            }
        }
        #endregion

        private ConcurrentDictionary<object, Data.DefaultSchemeWData> _itemsData = new ConcurrentDictionary<object, Data.DefaultSchemeWData>();

        public IDictionary<TItem, Data.DefaultSchemeWData> GetItemsFields<TItem>(IEnumerable<TItem> items) where TItem : Items.ItemBase
        {
            var measure = new MeasureTime();
            var measure2 = new MeasureTime();
            TimeSpan spanIterate = TimeSpan.Zero;
            TimeSpan spanQuery = TimeSpan.Zero;

            try
            {
                var collection = new Dictionary<TItem, Data.DefaultSchemeWData>();
                var items2 = new Dictionary<int, List<TItem>>();
                var ids = new List<int>();

                lock (SyncRoot)
                {
                    var ca = Cache;
                }

                int? IdItemTypeFirst = null;
                foreach (var item in items)
                {
                    var schemeItem = Scheme.SchemeItemAttribute.GetValueFromObject(item);
                    var IdItemType = Items.ItemTypeAttribute.GetValueFromObject(item);

                    if (IdItemTypeFirst == null) IdItemTypeFirst = IdItemType;
                    else if (IdItemTypeFirst.Value != IdItemType) throw new NotSupportedException("Список items должен содержать элементы с одинаковым IdItemType.");

                    var defaultSchemeToItem = Cache[schemeItem];

                    var fieldsData = Proxy.ProxyHelper.CreateTypeObjectFromParent<Data.DefaultSchemeWData, Field.IField>(defaultSchemeToItem);
                    fieldsData._schemeItemSource = schemeItem;
                    //var schemes = new Dictionary<int, Data.SchemeWData>();
                    //foreach (var scheme in fieldsData.Default.Schemes)
                    //{
                    //    var scheme = 
                    //}
                    //fieldsData.Schemes = new System.Collections.ObjectModel.ReadOnlyDictionary<int, Data.SchemeWData>(schemes);

                    var id = item.ID;
                    collection[item] = fieldsData;
                    if (!items2.ContainsKey(id)) items2[id] = new List<TItem>();
                    items2[id].AddIfNotExists(item);
                    if (!ids.Contains(id)) ids.Add(id);
                }
                spanIterate = measure2.Calculate();

                if (collection.Count > 0)
                {
                    var IdModule = this.Module.ID;

                    using (var db = this.CreateUnitOfWork())
                    {
                        var _ids = ids.ToArray();
                        var values = (from p in db.CustomFieldsDatas.AsNoTracking()
                                      join fi in db.CustomFieldsFields.AsNoTracking() on p.IdField equals fi.IdField
                                      where ids.Contains(p.IdItem) && p.IdItemType == IdItemTypeFirst.Value && fi.IdModule == IdModule && fi.Block == 0
                                      select p);

                        foreach (var res in values)
                        {
                            items2[res.IdItem].ForEach(item =>
                            {
                                var field = collection[item][res.IdField];
                                if (field != null) field.AddValue(res);
                            });
                        }
                    }
                }
                spanQuery = measure2.Calculate();

                return collection;
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(Journaling.EventType.Error, "Ошибка получения полей", $"{items?.Count()} items, {measure}ms all", ex);
                return null;
            }
        }

        public Data.DefaultSchemeWData GetItemFields<TItem>(TItem item) where TItem : Items.ItemBase
        {
            var fields = GetItemsFields(new List<TItem>() { item });
            return fields.Select(x=>x.Value).FirstOrDefault();
        }

        public IScheme<Field.IField> getSchemeFullByItem2(int itemID, int itemType, uint? schemeID = null)
        {
            try
            {
                DefaultScheme defaultScheme = null;
                var schemeItem = new SchemeItem(itemID, itemType);
                if (Cache.TryGetValue(schemeItem, out defaultScheme))
                {
                    if (!schemeID.HasValue || schemeID.Value == 0) return defaultScheme;

                    if (defaultScheme.Schemes.ContainsKey(schemeID.Value)) return defaultScheme.Schemes[schemeID.Value];
                    // todo else setError(string.Format("Для контейнера '{0}' не найдена схема №{1}", schemeItem, schemeID.Value));
                }
                // todo else setError("Схема по-умолчанию не найдена для контейнера " + schemeItem.ToString());
            }
            catch (Exception ex)
            {
                // todo setError(ex.Message);
            }
            return null;
        }

        #region Проверка и сохранение данных
        public void SaveItemFields<TItem>(TItem item) where TItem : Items.ItemBase
        {
            if (item._fields != null)
            {
                var IdItemType = Items.ItemTypeAttribute.GetValueFromObject(item);

                //using (var scope = DB.CreateScope())
                using (var db = new UnitOfWork<CustomFieldsData, CustomFieldsField>())
                using (var scope = db.CreateScope())
                {
                    (from d in db.Repo1
                     join f in db.Repo2 on d.IdField equals f.IdField
                     where f.IdModule == this.Module.ID && d.IdItem == item.ID && d.IdItemType == IdItemType
                     select d).Delete();

                    foreach (var field in item._fields.Values)
                    {
                        if (field._values != null)
                            foreach (var value in field._values)
                            {
                                int IdFieldValue = 0;
                                if (!int.TryParse(value.ToString(), out IdFieldValue)) IdFieldValue = 0;

                                db.Repo1.Add(new CustomFieldsData()
                                {
                                    IdField = field.IdField,
                                    IdFieldValue = field.IdValueType == Field.FieldValueType.KeyFromSource ? IdFieldValue : 0,
                                    FieldValue = field.IdValueType == Field.FieldValueType.KeyFromSource ? "" : value.ToString(),
                                    IdItem = item.ID,
                                    IdItemType = IdItemType,
                                    DateChange = DateTime.Now.Timestamp()
                                });
                            }
                    }

                    db.SaveChanges<CustomFieldsData>();

                    scope.Commit();
                }
            }
        }

        #endregion

        /// <summary>
        /// Возвращает поле, имеющее указанный <paramref name="alias"/> (см. <see cref="Field.IField.alias"/>), для связанного модуля. Если полей с таким alias несколько, вернет первое добавленное в базу.
        /// </summary>
        /// <returns>Возвращает объект поля, либо null, если поле не найдено или произошла ошибка.</returns>
        public Field.IField GetFieldByAlias(string alias)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var q_fields = db.CustomFieldsFields.Where(x => x.alias == alias && x.Block == 0).Include(x => x.data);
                    return q_fields.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(Journaling.EventType.Error, "Ошибка при получении поля", $"alias={alias}.", ex);
                return null;
            }
        }
        

        /// <summary>
        /// Возвращает поле, имеющее указанный идентификатор <paramref name="idField"/>, для связанного модуля.
        /// </summary>
        /// <returns>Возвращает объект поля, либо null, если поле не найдено или произошла ошибка.</returns>
        public Field.IField GetFieldByID(int idField)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var q_fields = db.CustomFieldsFields.Where(x => x.IdField == idField && x.Block == 0).Include(x => x.data);
                    return q_fields.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(Journaling.EventType.Error, "Ошибка при получении поля", $"idField={idField}.", ex);
                return null;
            }
        }

        //    /**
        //    * Возвращает поле, имеющее указанный номер, для связанного модуля.
        //    * 
        //    * @param mixed $IdField
        //    */
        //    public final function getFieldByID($IdField)
        //    {
        //        $this.Error = null;

        //        if (!self::isInt($IdField)) throw new Exception("Номер поля должен быть числом");

        //        $field = null;

        //        $sql = DataManager.executeQuery("
        //            SELECT CustomFieldsField.*, CustomFieldsValue.IdFieldValue, CustomFieldsValue.FieldValue, CustomFieldsValue.`Order` as FieldValueOrder
        //            FROM CustomFieldsField
        //            LEFT JOIN CustomFieldsValue ON CustomFieldsValue.IdField = CustomFieldsField.field_id
        //            WHERE CustomFieldsField.field_id='$IdField' AND
        //                    CustomFieldsField.field_module1='".$this.getModuleID()."' AND 
        //                    CustomFieldsField.status=1 AND 
        //                    CustomFieldsField.Block=0
        //            ORDER BY CustomFieldsValue.`Order` ASC
        //        ");
        //        while ($res = DataManager.getSelectedRecord($sql)) $field = $this.makeFieldRow($res, $field);

        //        if ($field == null) $this.Error = "Поле не найдено";

        //        return $field;
        //    }

        //    /**
        //    * Возвращает все поля для связанного модуля.
        //    */
        //    public final function getFieldsAll()
        //    {
        //        $this.Error = null;

        //        $fields = array();

        //        $sql = DataManager.executeQuery("
        //            SELECT CustomFieldsField.*, CustomFieldsValue.IdFieldValue, CustomFieldsValue.FieldValue, CustomFieldsValue.`Order` as FieldValueOrder
        //            FROM CustomFieldsField
        //            LEFT JOIN CustomFieldsValue ON CustomFieldsValue.IdField = CustomFieldsField.field_id
        //            WHERE   CustomFieldsField.field_module1='".$this.getModuleID()."' AND 
        //                    CustomFieldsField.status=1 AND 
        //                    CustomFieldsField.Block=0
        //            ORDER BY CustomFieldsValue.`Order` ASC
        //        ");
        //        while ($res = DataManager.getSelectedRecord($sql)) 
        //        {
        //            $field = null;
        //            if (isset($fields[$res['field_id"]])) $field = $fields[$res['field_id"]];
        //            $fields[$res['field_id"]] = $this.makeFieldRow($res, $field);
        //        }

        //        return $fields;
        //    }

    }
}
