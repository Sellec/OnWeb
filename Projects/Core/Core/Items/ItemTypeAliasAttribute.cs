using System;

namespace OnWeb.Core.Items
{
    /// <summary>
    /// Тип, помеченный данным атрибутом, будет восприниматься в <see cref="ItemTypeFactory"/> как тип <see cref="ItemTypeAliasAttribute.AliasType"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ItemTypeAliasAttribute : Attribute
    {
        /// <summary>
        /// Создает новый экземпляр конструктора с указанным ссылаемым типом.
        /// </summary>
        /// <param name="aliasType"></param>
        public ItemTypeAliasAttribute(Type aliasType)
        {
            if (aliasType == null) throw new ArgumentNullException();
            this.AliasType = aliasType;
        }

        /// <summary>
        /// Тип, в качестве которого в <see cref="ItemTypeFactory"/> будет восприниматься тип, помечаемый атрибутом.
        /// </summary>
        public Type AliasType{ get; private set; }

    }
}
