namespace OnWeb.Core.ModuleExtensions.CustomFields.Scheme
{
    /// <summary>
    /// Контейнер схемы. Указывает идентификатор и тип объекта, к которому привязаны поля схемы.
    /// Для подробностей см. <see cref="ExtensionCustomsFields"/>.
    /// </summary>
    public class SchemeItem
    {
        public SchemeItem(int IdItem, int IdItemType)
        {
            this.IdItem = IdItem;
            this.IdItemType = IdItemType;
        }

        /// <summary>
        /// Идентификатор контейнера схемы.
        /// </summary>
        public int IdItem { get; set; }

        /// <summary>
        /// Тип контейнера схемы.
        /// </summary>
        public int IdItemType { get; set; }

        public override int GetHashCode()
        {
            return string.Format("{0}_{1}", IdItemType, IdItem).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is SchemeItem) return (obj as SchemeItem).IdItem == IdItem && (obj as SchemeItem).IdItemType == IdItemType;
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return string.Format("IdItem={0}, IdItemType={1}", IdItem, IdItemType);
        }

        ///// <summary>
        ///// Контейнер схемы по-умолчанию, т.е. контейнер верхнего уровня для любой схемы и любого другого уровня.
        ///// Подробнее см. <see cref="ExtensionCustomsFields"/>.
        ///// </summary>
        //public static SchemeItem Default = new SchemeItem(0, null);
    }


}
