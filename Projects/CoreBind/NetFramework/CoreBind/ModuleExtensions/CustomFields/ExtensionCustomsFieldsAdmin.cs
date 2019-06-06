using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Core.ModuleExtensions.CustomFields
{
    using Core.DB;
    using Core.Modules;
    using CoreBind.Modules;
    using DB;
    using Modules.Extensions;
    using Scheme;

    /// <summary>
    /// Админский класс расширения пользовательских полей. 
    /// </summary>
    [ModuleExtension("CustomFields", true)]
    public class ExtensionCustomsFieldsAdmin : ExtensionCustomsFieldsBase
    {
        public ExtensionCustomsFieldsAdmin(ModuleCore moduleObject)
            : base(moduleObject)
        {
        }

        #region Вспомогательное
        public override Types.NestedLinkCollection getAdminMenu()
        {
            return new Types.NestedLinkCollection(Types.NestedLinkSimple.RelativeToModule("fields", "Управление полями", this.Module));
        }

        /// <summary>
        /// Возвращает список схем, зарегистрированных для текущего модуля.
        /// </summary>
        /// <returns></returns>
        public IDictionary<uint, string> GetSchemeList()
        {
            var schemes = new Dictionary<uint, string>() { { 0, "По-умолчанию" } };

            using (var db = this.CreateUnitOfWork())
            {
                var idmodule = this.GetModuleID();
                foreach (var res in (from p in db.CustomFieldsSchemes where p.IdModule == idmodule && p.IdScheme > 0 orderby p.NameScheme select p))
                    schemes[(uint)res.IdScheme] = res.NameScheme;
            }

            return schemes;
        }

        /// <summary>
        /// Возвращает список контейнеров схем, зарегистрированных для текущего модуля.
        /// </summary>
        /// <returns></returns>
        public IDictionary<ItemType, IDictionary<SchemeItem, string>> GetSchemeItemsList()
        {
            var itemsGroups = new Dictionary<ItemType, IDictionary<SchemeItem, string>>();

            foreach (var itemType in this.Module.GetItemTypes())
            {
                var items = new Dictionary<SchemeItem, string>();

                if (itemType.IdItemType != Items.ItemTypeFactory.CategoryType.IdItemType)
                {
                    items[new SchemeItem(0, itemType.IdItemType)] = "По-умолчанию";
                }
                else
                {
                    var _itemsPre = this.Module.GetItems(itemType.IdItemType, Types.eSortType.Name);
                    var _items = _itemsPre != null ? _itemsPre.GetSimplifiedHierarchy() : new Types.NestedListCollectionSimplified();

                    if (_items != null && _items.Count() > 0)
                        foreach (var res in _items)
                            items[new SchemeItem(res.Key.ID, itemType.IdItemType)] = res.Value;
                    else
                        items[new SchemeItem(0, itemType.IdItemType)] = "По-умолчанию";
                }

                itemsGroups[itemType] = items;
            }

            return itemsGroups;
        }
        #endregion

        #region Контроллер
        [ModuleAction("fields")]
        public ActionResult FieldsList()
        {
            UpdateCache();

            var schemes = GetSchemeList();
            var schemeItems = GetSchemeItemsList();

            var id = this.GetModuleID();

            using (var db = this.CreateUnitOfWork())
            {
                var fields = (from p in db.CustomFieldsFields
                              where p.Block == 0 && p.IdModule == id
                              orderby p.name
                              select p).ToDictionary(x => x.IdField, x => x as Field.IField);

                return Controller.display("ModuleExtensions/CustomFields/Design/SchemesEdit.cshtml", new Model.Fields
                {
                    FieldsList = fields,
                    SchemeItems = schemeItems,
                    Schemes = schemes,
                    AllowSchemesManage = AllowSchemeManage
                });
            }
        }

        [ModuleAction("fieldsItem")]
        public ActionResult ContainerItem(int idSchemeItem = 0, int idSchemeItemType = ModuleCore.CategoryType)
        {
            var schemeItem = new SchemeItem(idSchemeItem, idSchemeItemType);

            using (var db = this.CreateUnitOfWork())
            {
                var fields = db.CustomFieldsFields.Where(x => x.IdModule == Module.ID && x.Block == 0).ToDictionary(x => x.IdField, x => x);

                var schemes = db.CustomFieldsSchemes.
                    Where(x => x.IdModule == Module.ID && x.IdScheme > 0).
                    OrderBy(x => x.NameScheme).
                    ToDictionary(x => (uint)x.IdScheme, x => new Model.SchemeContainerItem.Scheme() { Name = x.NameScheme, Fields = new Dictionary<int, Field.IField>() });

                schemes[0] = new Model.SchemeContainerItem.Scheme() { Name = "По-умолчанию", Fields = new Dictionary<int, Field.IField>() };

                var sql = (from datas in db.CustomFieldsSchemeDatas
                           where datas.IdModule == Module.ID && datas.IdItemType == schemeItem.IdItemType && datas.IdSchemeItem == schemeItem.IdItem
                           group new { datas.IdField, datas.Order } by datas.IdScheme into gr
                           select new { IdScheme = gr.Key, Fields = gr.OrderBy(x => x.Order).Select(x => x.IdField).ToList() });

                var fieldsWithSchemes = sql.
                                         ToList().
                                         ToDictionary(x => (uint)x.IdScheme,
                                                      x => x.Fields.GroupBy(IdField => IdField).
                                                                Select(group => fields.GetValueOrDefault(group.Key)).
                                                                Where(field => field != null).
                                                                ToDictionary(field => field.IdField, field => field as Field.IField)
                                         );

                var model = new Model.SchemeContainerItem() { SchemeItem = schemeItem };

                if (fieldsWithSchemes.ContainsKey(0)) schemes[0].Fields = fieldsWithSchemes[0];
                foreach (var p in schemes)
                {
                    model.Schemes[p.Key] = p.Value;

                    if (fieldsWithSchemes.ContainsKey(p.Key)) p.Value.Fields = fieldsWithSchemes[p.Key];
                    int i = 0;
                    p.Value.Fields = p.Value.Fields.Where(x => schemes[0].Fields.ContainsKey(x.Key)).ToDictionary(x => i++, x => x.Value);
                }

                return Controller.display("ModuleExtensions/CustomFields/Design/Item.cshtml", model);
            }
        }

        [ModuleAction("fieldsItemSave")]
        public JsonResult ContainerItemSave(int idSchemeItem = 0, int idSchemeItemType = 0, Dictionary<string, int[]> model = null)
        {
            var result = Controller.JsonAnswer();

            try
            {
                var schemeItem = new SchemeItem(idSchemeItem, idSchemeItemType);

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope())
                {
                    db.CustomFieldsSchemeDatas.Where(x => x.IdModule == this.Module.ID && x.IdSchemeItem == schemeItem.IdItem && x.IdItemType == schemeItem.IdItemType).Delete();

                    int k = 0;
                    var modelPrepared = model != null ? model.Where(x => int.TryParse(x.Key, out k)).ToDictionary(x => uint.Parse(x.Key), x => new List<int>(x.Value)) : null;
                    if (modelPrepared != null)
                    {
                        if (!modelPrepared.ContainsKey(0)) modelPrepared[0] = new List<int>();
                        foreach (var key in modelPrepared.Keys.ToList())
                        {
                            if (key == 0) continue;
                            var list = modelPrepared[key].Where(x => modelPrepared[0].Contains(x)).ToList();
                            modelPrepared[key] = list;
                        }

                        foreach (var pair in modelPrepared)
                        {
                            int i = 0;
                            foreach (var field in pair.Value)
                                db.CustomFieldsSchemeDatas.Add(new CustomFieldsSchemeData()
                                {
                                    IdModule = this.Module.ID,
                                    IdScheme = (int)pair.Key,
                                    IdField = field,
                                    IdSchemeItem = schemeItem.IdItem,
                                    IdItemType = schemeItem.IdItemType,
                                    Order = i++
                                });
                        }

                        db.SaveChanges();
                    }

                    scope.Commit();

                    result.Message = "Сохранение расположения полей прошло успешно.";
                    result.Success = true;
                }
                UpdateCache();
            }
            catch (Exception ex) { result.Message = ex.Message; }

            return Controller.ReturnJson(result);
        }

        [ModuleAction("fields_scheme_add")]
        public JsonResult SchemeAdd(string schemeName = null)
        {
            var result = Controller.JsonAnswer<uint>();

            try
            {
                if (Request.Form.HasKey(nameof(schemeName))) schemeName = Request.Form[nameof(schemeName)];

                if (!this.AllowSchemeManage) result.Message = "Управление схемами отключено для модуля.";
                else if (string.IsNullOrEmpty(schemeName)) result.Message = "Не указано название схемы!";
                else if (!schemeName.isOneStringTextOnly()) result.Message = "Некорректно указано название схемы!";
                else
                {
                    using (var db = this.CreateUnitOfWork())
                    {
                        var data = new CustomFieldsScheme()
                        {
                            IdModule = this.Module.ID,
                            NameScheme = schemeName
                        };
                        db.CustomFieldsSchemes.Add(data);

                        if (db.SaveChanges() > 0)
                        {
                            result.Message = "Схема добавлена.";
                            result.Success = true;
                            result.Data = (uint)data.IdScheme;
                            UpdateCache();
                        }
                        else result.Message = "По неизвестной причине схему не получилось добавить.";
                    }
                }
            }
            catch (Exception ex) { result.Message = ex.Message; }

            return Controller.ReturnJson(result);
        }

        [ModuleAction("fields_scheme_delete")]
        public JsonResult SchemeDelete(int IdScheme)
        {
            var result = Controller.JsonAnswer<int>();

            try
            {
                if (!this.AllowSchemeManage) result.Message = "Управление схемами отключено для модуля.";
                else if (IdScheme == 0) result.Message = "Схема \"По-умолчанию\" не удаляется.";
                else
                {
                    using (var db = this.CreateUnitOfWork())
                    using (var scope = db.CreateScope())
                    {
                        var data = db.CustomFieldsSchemes.Where(x => x.IdScheme == IdScheme).FirstOrDefault();
                        if (data == null) throw new Exception(string.Format("Схема с номером '{0}' не найдена.", IdScheme));

                        db.CustomFieldsSchemeDatas.Where(x => x.IdScheme == IdScheme).Delete();
                        db.CustomFieldsSchemes.Where(x => x.IdScheme == IdScheme).Delete();

                        if (db.SaveChanges() > 0)
                        {
                            result.Message = "Схема удалена.";
                            result.Success = true;
                            scope.Commit();
                            UpdateCache();
                        }
                        else result.Message = "По неизвестной причине схему не получилось удалить.";
                    }
                }
            }
            catch (Exception ex) { result.Message = ex.Message; }

            return Controller.ReturnJson(result);
        }

        [ModuleAction("fieldEdit")]
        public ActionResult FieldEdit(int IdField = 0)
        {
            CustomFieldsField data = null;
            if (IdField > 0)
            {
                using (var db = this.CreateUnitOfWork())
                {
                    data = db.CustomFieldsFields.Where(x => x.IdField == IdField).Include(x => x.data).FirstOrDefault();
                    if (data == null) throw new Exception("Такое поле не найдено в базе данных!");
                    if (data.IdModule != this.Module.ID)
                    {
                        var module = AppCore.GetModulesManager().GetModule(data.IdModule);
                        if (module == null) throw new Exception("Это поле относится к другому модулю.");
                        else throw new Exception(string.Format("Это поле относится к модулю '{0}'.", module.Caption));
                    }
                }
            }
            else
            {
                data = new DB.CustomFieldsField() { IdFieldType = 0, IdModule = this.Module.ID };
            }

            return Controller.display("ModuleExtensions/CustomFields/Design/FieldEdit.cshtml", new Model.FieldEdit(data));
        }

        [ModuleAction("fieldSave")]
        public JsonResult FieldSave(Model.FieldEdit model = null)
        {
            var result = Controller.JsonAnswer();

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    CustomFieldsField data = null;
                    if (model.IdField > 0)
                    {
                        data = db.CustomFieldsFields.Where(x => x.IdField == model.IdField).Include(x => x.data).FirstOrDefault();
                        if (data == null) throw new Exception("Такое поле не найдено в базе данных!");
                        if (data.IdModule != this.Module.ID)
                        {
                            var module = AppCore.GetModulesManager().GetModule(data.IdModule);
                            if (module == null) throw new Exception("Это поле относится к другому модулю.");
                            else throw new Exception(string.Format("Это поле относится к модулю '{0}'.", module.Caption));
                        }
                    }
                    else
                    {
                        data = new DB.CustomFieldsField() { IdFieldType = 0, IdModule = this.Module.ID };
                        db.CustomFieldsFields.Add(data);
                    }

                    if (ModelState.ContainsKeyCorrect(nameof(model.name))) data.name = model.name;
                    if (ModelState.ContainsKeyCorrect(nameof(model.nameAlt))) data.nameAlt = model.nameAlt;
                    if (ModelState.ContainsKeyCorrect(nameof(model.alias))) data.alias = model.alias;
                    if (ModelState.ContainsKeyCorrect(nameof(model.formatCheck))) data.formatCheck = model.formatCheck;
                    if (ModelState.ContainsKeyCorrect(nameof(model.IdFieldType))) data.IdFieldType = model.IdFieldType;
                    if (ModelState.ContainsKeyCorrect(nameof(model.IsValueRequired))) data.IsValueRequired = model.IsValueRequired;
                    if (ModelState.ContainsKeyCorrect(nameof(model.IdValueType))) data.IdValueType = model.IdValueType;
                    if (ModelState.ContainsKeyCorrect(nameof(model.size))) data.size = model.size;
                    if (ModelState.ContainsKeyCorrect(nameof(model.nameEnding))) data.nameEnding = model.nameEnding;

                    using (var scope = db.CreateScope())
                    {
                        var entry = db.GetState(data);
                        if (entry == ItemState.Detached) throw new Exception("Невозможно найти поле для сохранения.");
                        else if (entry.In(ItemState.Modified, ItemState.Added))
                        {
                            data.DateChange = DateTime.Now.Timestamp();
                            data.Block = 0;

                            if (db.SaveChanges<CustomFieldsField>() == 0) throw new Exception("Сохранение поля провалилось!");
                        }

                        if (Request.Form.HasKey("values"))
                        {
                            var t = DateTime.Now.Timestamp();
                            var vals = Request.Form.TryGetValue<CustomFieldsValue[]>("values");

                            if (vals != null)
                            {
                                foreach (var d in vals)
                                {
                                    d.IdField = data.IdField;
                                    //d.Field = data;
                                    d.DateChange = t;
                                }

                                db.CustomFieldsValues.AddOrUpdate(vals);
                            }
                            else vals = new CustomFieldsValue[0];

                            var values = vals.ToList();

                            if (values.Count > 0)
                            {
                                if (db.SaveChanges<CustomFieldsValue>() == 0) throw new Exception("Не удалось сохранить варианты значений поля");
                            }

                            data.data = data.FieldType.CreateValuesCollection(data, values);

                            var keys = values.Select(x => x.IdFieldValue);

                            var removeExistsValue = new Dictionary<int, string>();
                            var removeExistsObjects = new Dictionary<int, List<int>>();

                            var sql = (from p in db.CustomFieldsValues
                                       join d in db.CustomFieldsDatas on p.IdFieldValue equals d.IdFieldValue
                                       where p.IdField == data.IdField && !keys.Contains(p.IdFieldValue)
                                       select new { p, d });

                            foreach (var res in sql)
                            {
                                removeExistsValue[res.p.IdFieldValue] = res.p.FieldValue;
                                if (!removeExistsObjects.ContainsKey(res.p.IdFieldValue)) removeExistsObjects[res.p.IdFieldValue] = new List<int>();
                                if (!removeExistsObjects[res.p.IdFieldValue].Contains(res.d.IdItem)) removeExistsObjects[res.p.IdFieldValue].Add(res.d.IdItem);
                            }

                            if (removeExistsValue.Count > 0)
                            {
                                var removeTexts = new Dictionary<int, string>();
                                foreach (var pair in removeExistsValue)
                                    removeTexts[pair.Key] = " - №" + pair.Key + " \"" + pair.Value + "\" у объектов №" + string.Join(", №", removeExistsObjects[pair.Key]);

                                throw new Exception("Некоторые значения, которые вы пытаетесь удалить, указаны у следующих объектов:\r\n" + string.Join(", \r\n", removeTexts.Values));
                            }

                            //todo подозрение, что неправильный запрос, может очистить всю таблицу.
                            //DB.CustomFieldsValues.Where(x => !keys.Contains(x.IdFieldValue)).Delete();
                        }

                        result.Message = "Сохранение поля прошло успешно!";
                        result.Success = true;
                        result.Data = data;

                        scope.Commit();
                    }

                    result.Success = true;
                }
            }
            catch (OnUtils.Data.Validation.EntityValidationException ex) { result.Message = ex.CreateComplexMessage(); }
            catch (Exception ex) { result.Message = ex.Message; }

            return Controller.ReturnJson(result);
        }

        [ModuleAction("fieldDelete")]
        public JsonResult FieldDelete(int IdField = 0)
        {
            var result = Controller.JsonAnswer();

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var data = db.CustomFieldsFields.Where(x => x.IdField == IdField).Include(x => x.data).FirstOrDefault();
                    if (data == null) throw new Exception("Такое поле не найдено в базе данных!");
                    if (data.IdModule != this.Module.ID)
                    {
                        var module = AppCore.GetModulesManager().GetModule(data.IdModule);
                        if (module == null) throw new Exception("Это поле относится к другому модулю.");
                        else throw new Exception(string.Format("Это поле относится к модулю '{0}'.", module.Caption));
                    }

                    using (var scope = db.CreateScope())
                    {
                        db.DeleteEntity(data);
                        if (db.SaveChanges<CustomFieldsField>() > 0)
                        {
                            db.CustomFieldsValues.Where(x => x.IdField == data.IdField).Delete();
                            db.CustomFieldsDatas.Where(x => x.IdField == data.IdField).Delete();

                            result.Message = "Удаление поля прошло успешно.";
                            result.Success = true;

                            scope.Commit();
                        }
                        else throw new Exception("Не получилось удалить поле.");
                    }
                }
            }
            catch (Exception ex) { result.Message = ex.Message; }

            return Controller.ReturnJson(result);
        }

        #endregion

        protected ModuleControllerBase Controller
        {
            get => ControllerBase as ModuleControllerBase;
        }

        protected System.Web.HttpRequest Request
        {
            get => System.Web.HttpContext.Current.Request;
        }

        protected ModelStateDictionary ModelState
        {
            get => Controller?.ModelState;
        }
    }
}
