using OnUtils.Architecture.AppCore;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class HiddenSimpleMultiLineFieldTypeRender : CoreComponentBase<ApplicationCore>, ICustomFieldRender<HiddenSimpleMultiLineFieldType>
    {
        MvcHtmlString ICustomFieldRender<HiddenSimpleMultiLineFieldType>.RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            htmlAttributes["style"] = "display:none;";
            if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

            var value = (field as Data.FieldData)?.ToString();
            return html.TextArea($"fieldValue_{field.IdField}", value, htmlAttributes);
        }

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion
    }
}
