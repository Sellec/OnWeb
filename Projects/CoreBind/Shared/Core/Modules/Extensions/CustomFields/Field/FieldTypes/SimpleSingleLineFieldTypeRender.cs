using OnUtils.Application.Modules.Extensions.CustomFields.Data;
using OnUtils.Application.Modules.Extensions.CustomFields.Field;
using OnUtils.Application.Modules.Extensions.CustomFields.Field.FieldTypes;
using System.Collections.Generic;
#if NETFULL
using System.Web.Mvc;
using System.Web.Mvc.Html;
#elif NETCORE
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MvcHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace OnWeb.Core.Modules.Extensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class SimpleSingleLineFieldTypeRender : CoreComponentBase, ICustomFieldRender<SimpleSingleLineFieldType>
    {
        MvcHtmlString ICustomFieldRender<SimpleSingleLineFieldType>.RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            var value = (field as FieldData)?.ToString();
            if (htmlAttributes == null || htmlAttributes.IsReadOnly) htmlAttributes = new Dictionary<string, object>(htmlAttributes);
            if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

            switch (field.IdValueType)
            {
                case FieldValueType.Email:
                    htmlAttributes["type"] = "email";
                    break;

                case FieldValueType.Phone:
                    htmlAttributes["type"] = "phone";
                    break;

                case FieldValueType.URL:
                    htmlAttributes["type"] = "URL";
                    break;

                case FieldValueType.Boolean:
                    return html.CheckBox($"fieldValue_{field.IdField}", value == true.ToString(), htmlAttributes);

            }

#if NETFULL
            return html.TextBox($"fieldValue_{field.IdField}", value, htmlAttributes);
#elif NETCORE
            return html.TextBox($"fieldValue_{field.IdField}", value, null, htmlAttributes);
#endif
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
