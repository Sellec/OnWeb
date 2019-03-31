using System.Collections.Generic;

namespace System.Web.Mvc.Html
{
    using OnWeb.Core.Items;
    using IEnumerableItemBase = IEnumerable<OnWeb.Core.Items.ItemBase>;
    using IEnumerableSelectListItem = IEnumerable<SelectListItem>;

    /// <summary>
    /// </summary>
    public static class ListBoxHtmlExtensions
    {
        /// <summary>
        /// См. <see cref="SelectExtensions.ListBox(HtmlHelper, string, IEnumerableSelectListItem)"/>.
        /// Отличается тем, что автоматически преобразует <see cref="IEnumerableItemBase"/> в <see cref="IEnumerableSelectListItem"/> путем вызова <see cref="SelectListItemExtension.AsSelectListItem(IEnumerableItemBase)"/>.
        /// </summary>
        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<ItemBase> selectList)
        {
            return htmlHelper.ListBox(name, selectList.AsSelectListItem());
        }

        /// <summary>
        /// См. <see cref="SelectExtensions.ListBox(HtmlHelper, string, IEnumerableSelectListItem, object)"/>.
        /// Отличается тем, что автоматически преобразует <see cref="IEnumerableItemBase"/> в <see cref="IEnumerableSelectListItem"/> путем вызова <see cref="SelectListItemExtension.AsSelectListItem(IEnumerableItemBase)"/>.
        /// </summary>
        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<ItemBase> selectList, object htmlAttributes)
        {
            return htmlHelper.ListBox(name, selectList.AsSelectListItem(), htmlAttributes);
        }

        /// <summary>
        /// См. <see cref="SelectExtensions.ListBox(HtmlHelper, string, IEnumerableSelectListItem, IDictionary{string, object})"/>.
        /// Отличается тем, что автоматически преобразует <see cref="IEnumerableItemBase"/> в <see cref="IEnumerableSelectListItem"/> путем вызова <see cref="SelectListItemExtension.AsSelectListItem(IEnumerableItemBase)"/>.
        /// </summary>
        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<ItemBase> selectList, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ListBox(name, selectList.AsSelectListItem(), htmlAttributes);
        }

        /// <summary>
        /// См. <see cref="SelectExtensions.ListBox(HtmlHelper, string, IEnumerableSelectListItem)"/>.
        /// Отличается тем, что автоматически преобразует <see cref="IEnumerableItemBase"/> в <see cref="IEnumerableSelectListItem"/> путем вызова <see cref="SelectListItemExtension.AsSelectListItem(IEnumerableItemBase)"/>.
        /// </summary>
        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<ItemBase> selectList, IEnumerable<object> selectedValues)
        {
            return htmlHelper.ListBox(name, selectList.SelectListWithSelected(selectedValues));
        }


        /// <summary>
        /// См. <see cref="SelectExtensions.ListBox(HtmlHelper, string, IEnumerableSelectListItem, object)"/>.
        /// Отличается тем, что автоматически преобразует <see cref="IEnumerableItemBase"/> в <see cref="IEnumerableSelectListItem"/> путем вызова <see cref="SelectListItemExtension.AsSelectListItem(IEnumerableItemBase)"/>.
        /// </summary>
        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<ItemBase> selectList, IEnumerable<object> selectedValues, object htmlAttributes)
        {
            return htmlHelper.ListBox(name, selectList.SelectListWithSelected(selectedValues), htmlAttributes);
        }

        /// <summary>
        /// См. <see cref="SelectExtensions.ListBox(HtmlHelper, string, IEnumerableSelectListItem, IDictionary{string, object})"/>.
        /// Отличается тем, что автоматически преобразует <see cref="IEnumerableItemBase"/> в <see cref="IEnumerableSelectListItem"/> путем вызова <see cref="SelectListItemExtension.AsSelectListItem(IEnumerableItemBase)"/>.
        /// </summary>
        public static MvcHtmlString ListBox(this HtmlHelper htmlHelper, string name, IEnumerable<ItemBase> selectList, IEnumerable<object> selectedValues, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.ListBox(name, selectList.SelectListWithSelected(selectedValues), htmlAttributes);
        }

    }
}
