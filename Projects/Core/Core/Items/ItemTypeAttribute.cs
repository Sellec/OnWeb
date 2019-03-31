using System;
using System.Collections.Generic;
using System.Reflection;

namespace OnWeb.Core.Items
{
    using ModuleExtensions.CustomFields.Scheme;
    using Modules;

    /// <summary>
    /// Описывает поле или свойство, являющееся ключом для получения контейнера схемы <see cref="SchemeItem"/> для объекта. 
    /// Например, если пометить этим атрибутом свойство Category для Goods, то при поиске схемы полей для объекта Goods 
    /// будет использовано значение поля Category в качестве <see cref="SchemeItem.IdItem"/> и значение <see cref="IdItemType"/> в качестве <see cref="SchemeItem.IdItemType"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ItemTypeAttribute : Attribute
    {
        private static Dictionary<Type, int> _knownTypes = new Dictionary<Type, int>();

        /// <summary>
        /// Создает новый экземпляр атрибута с указанным типом объекта (см. <see cref="ItemTypeFactory.GetItemType(Type)"/>).
        /// </summary>
        public ItemTypeAttribute(int IdItemType = ModuleCore.ItemType)
        {
            this.IdItemType = IdItemType;
        }

        /// <summary>
        /// Тип контейнера схемы.
        /// </summary>
        public int IdItemType { get; private set; }

        /// <summary>
        /// Возвращает тип указанного объекта.
        /// </summary>
        public static int GetValueFromObject(object obj)
        {
            var t = obj.GetType();

            return GetValueFromType(t);
        }

        /// <summary>
        /// Возвращает тип указанного объекта.
        /// </summary>
        public static int GetValueFromType(Type t)
        {
            if (!_knownTypes.ContainsKey(t))
            {
                var attr = t.GetCustomAttribute<ItemTypeAttribute>(true);
                if (attr != null) _knownTypes[t] = attr.IdItemType;
            }

            if (_knownTypes.ContainsKey(t)) return _knownTypes[t];

            var itemType = ItemTypeFactory.GetItemType(t);
            if (itemType != null) return itemType.IdItemType;

            return ModuleCore.ItemType;
        }
    }
}
