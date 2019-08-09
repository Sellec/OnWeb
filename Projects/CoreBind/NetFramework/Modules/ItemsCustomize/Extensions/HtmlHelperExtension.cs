using OnUtils.Application.Modules.ItemsCustomize.Field;
using OnWeb.Modules.ItemsCustomize;
using System.Collections.Generic;
using System.Linq;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtension
    {
        #region LabelFor
        /// <summary>
        /// Возвращает элемент label с именем переданного поля. См. <see cref="IField.GetDisplayName"/>.
        /// </summary>
        public static MvcHtmlString LabelFor<TModel>(this HtmlHelper<TModel> html, IField field)
        {
            return html.LabelFor(field, null);
        }

        /// <summary>
        /// Возвращает элемент label с именем переданного поля. См. <see cref="IField.GetDisplayName"/>. 
        /// </summary>
        /// <param name="prefix">Если указано, то добавляется перед именем поля.</param>
        /// <param name="postfix">Если указано, то добавляется после имени поля.</param>
        public static MvcHtmlString LabelFor<TModel>(this HtmlHelper<TModel> html, IField field, object htmlAttributes, string prefix = null, string postfix = null)
        {
            return LabelHelper(html, html.ViewData.ModelMetadata, $"fieldValue_{field.IdField}", $"{prefix}{field.GetDisplayName()}{postfix}", HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        internal static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string labelText = null, IDictionary<string, object> htmlAttributes = null)
        {
            string arg_31_0 = labelText;
            if (labelText == null && (arg_31_0 = metadata.DisplayName) == null && (arg_31_0 = metadata.PropertyName) == null)
            {
                arg_31_0 = htmlFieldName.Split(new char[]
                {
                    '.'
                }).Last<string>();
            }
            string text = arg_31_0;
            if (string.IsNullOrEmpty(text))
            {
                return MvcHtmlString.Empty;
            }
            TagBuilder tagBuilder = new TagBuilder("label");
            tagBuilder.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
            tagBuilder.SetInnerText(text);
            tagBuilder.MergeAttributes<string, object>(htmlAttributes, true);
            return tagBuilder.ToMvcHtmlString(TagRenderMode.Normal);
        }
        #endregion

        #region EditorFor
        public static MvcHtmlString EditorFor<TModel>(this HtmlHelper<TModel> html, IField field)
        {
            return html.EditorFor(field, null);
        }

        public static MvcHtmlString EditorFor<TModel>(this HtmlHelper<TModel> html, IField field, object htmlAttributes, params object[] additionalParameters)
        {
            return html.EditorFor(field, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), additionalParameters);
        }

        public static MvcHtmlString EditorFor<TModel>(this HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            var module = (html.ViewDataContainer as OnWeb.CoreBind.Razor.IModuleProvider)?.Module;
            var appCore = module?.GetAppCore();

            var renderType = typeof(ICustomFieldRender<>).MakeGenericType(field.FieldType.GetType());
            var customFieldRender = appCore.Create<ICustomFieldRender<FieldType>>(renderType);
            return customFieldRender?.RenderHtmlEditor(html, field, htmlAttributes, additionalParameters);
        }

        #endregion

        internal static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode)
        {
            return new MvcHtmlString(tagBuilder.ToString(renderMode));
        }
    }
}