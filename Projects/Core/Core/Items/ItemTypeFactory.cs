using OnUtils.Data;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace OnWeb.Core.Items
{
    /// <summary>
    /// Предоставляет методы для работы с типами объектов.
    /// </summary>
    public class ItemTypeFactory //: ProvidersFactoryStartup<IItemTypeProvider, ItemTypeFactory>
    {
        /// <summary>
        /// Значение, обозначающее, что идентификатор типа объектов не найден.
        /// </summary>
        public const DB.ItemType NotFound = null;

        /// <summary>
        /// Значение, обозначающее идентификатор объектов.
        /// </summary>
        public static readonly DB.ItemType ItemType = new DB.ItemType() { IdItemType=Modules.ModuleCore.ItemType, NameItemType = "Объект внутри категории", UniqueKey= "ItemType" };

        /// <summary>
        /// Значение, обозначающее идентификатор категорий.
        /// </summary>
        public static readonly DB.ItemType CategoryType = new DB.ItemType() { IdItemType = Modules.ModuleCore.CategoryType, NameItemType = "Категория объектов", UniqueKey = "CategoryType" };

        private static Lazy<Tuple<DateTime, ConcurrentDictionary<string, DB.ItemType>>> _itemsTypes = null;

        private static Tuple<DateTime, ConcurrentDictionary<string, DB.ItemType>> ItemsTypesProvide()
        {
            var types = new ConcurrentDictionary<string, DB.ItemType>();

            using (var db = new UnitOfWork<DB.ItemType>())
            {
                db.Repo1.Where(x => !string.IsNullOrEmpty(x.UniqueKey)).ForEach(x => types[x.UniqueKey] = x);
            }

            var expires = DateTime.Now.AddMinutes(2);

            //Debug.WriteLineNoLog("ItemTypeFactory: generate new cache with {0} types, expires at {1}", types.Count, expires.ToString("yyyy-MM-dd HH:mm:ss"));

            return new Tuple<DateTime, ConcurrentDictionary<string, DB.ItemType>>(DateTime.Now.AddMinutes(2), types);
        }

        /// <summary>
        /// Возвращает тип объектов для идентификатора <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Идентификатор, для которого следует получить тип объектов.</param>
        public static DB.ItemType GetItemType(int type)
        {
            if (type <= 0) return NotFound;

            var _r = ItemTypes.Where(x => x.Value.IdItemType == type).Select(x => x.Value).FirstOrDefault();
            if (_r == null)
                using (var db = new UnitOfWork<DB.ItemType>())
                    _r = db.Repo1.Where(x => x.IdItemType == type).FirstOrDefault();

            if (_r != null) return _r;

            return NotFound;
        }

        /// <summary>
        /// Возвращает идентификатор указанного типа объектов <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Тип объектов, для которого следует получить идентификатор.</param>
        public static DB.ItemType GetItemType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type), "Следует указать тип объекта");

            var t = type;
            var aliasTypeAttribute = type.GetCustomAttribute<ItemTypeAliasAttribute>(true);
            if (aliasTypeAttribute != null) return GetItemType(aliasTypeAttribute.AliasType);

            var itemTypeAttribute = type.GetCustomAttribute<ItemTypeAttribute>(true);
            if (itemTypeAttribute != null) return GetItemType(itemTypeAttribute.IdItemType);


            if (t.FullName.StartsWith("System.Data.Entity.DynamicProxies.")) t = t.BaseType;

            return GetOrAdd(t.Name, "TYPEKEY_" + t.FullName, true);
        }

        /// <summary>
        /// Возвращает идентификатор для указанных ключа и названия типа.
        /// </summary>
        public static DB.ItemType GetItemType(string typeKey, string caption)
        {
            if (string.IsNullOrEmpty(caption)) throw new ArgumentNullException(nameof(caption), "Название типа не должно быть пустым.");
            if (string.IsNullOrEmpty(typeKey)) throw new ArgumentNullException(nameof(typeKey), "Ключ типа должен быть пустым.");
            return GetOrAdd(caption, "CUSTOMKEY_" + typeKey, true);
        }

        // todo понять, нужен ли этот метод.
        public static object GetItemOfType(int type, int IdItem)
        {
            return GetItemOfType(GetItemType(type), IdItem);
        }

        // todo понять, нужен ли этот метод.
        public static object GetItemOfType(DB.ItemType type, int IdItem)
        {
            throw new NotImplementedException();
            //if (type != NotFound)
            //    foreach (var provider in this.Providers)
            //    {
            //        if (provider.IsProvideType(type))
            //        {
            //            var item = provider.GetItemOfType(type, IdItem);
            //            if (item != null) return item;
            //        }
            //    }

            return null;
        }


        private static DB.ItemType GetOrAdd(string caption, string uniqueKey, bool registerIfNoFound)
        {
            var _r = ItemTypes.Where(x => x.Value.UniqueKey == uniqueKey).Select(x => x.Value).FirstOrDefault();
            if (_r == null)
            {
                using (var db = new UnitOfWork<DB.ItemType>())
                {
                    _r = db.Repo1.Where(x => x.UniqueKey == uniqueKey).FirstOrDefault();
                    if (_r == null && registerIfNoFound)
                    {
                        var r = new DB.ItemType() { NameItemType = caption, UniqueKey = uniqueKey };
                        db.Repo1.AddOrUpdate(x => x.UniqueKey, r);
                        db.SaveChanges();

                        _r = r;
                    }
                    ItemTypes[uniqueKey] = _r;
                }
            }

            return _r;
        }

        private static ConcurrentDictionary<string, DB.ItemType> ItemTypes
        {
            get
            {
                if (_itemsTypes == null || (_itemsTypes.IsValueCreated && _itemsTypes.Value.Item1 <= DateTime.Now)) _itemsTypes = new Lazy<Tuple<DateTime, ConcurrentDictionary<string, DB.ItemType>>>(ItemsTypesProvide);
                return _itemsTypes.Value.Item2;
            }
        }
    }
}
