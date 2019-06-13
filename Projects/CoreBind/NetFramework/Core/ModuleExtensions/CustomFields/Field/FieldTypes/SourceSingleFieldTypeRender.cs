using System.Collections.Generic;
using System.Linq;
using OnUtils.Architecture.AppCore;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
    using Core.ModuleExtensions.CustomFields.MetadataAndValues;

#pragma warning disable CS1591 // todo внести комментарии.
    public class SourceSingleFieldTypeRender : CoreComponentBase<ApplicationCore>, ICustomFieldRender<SourceSingleFieldType>
    {
        MvcHtmlString ICustomFieldRender<SourceSingleFieldType>.RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            var value = (field as Data.FieldData)?.Value?.ToString();

            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

            htmlAttributes = htmlAttributes.Where(x => x.Key.ToLower() != "size").ToDictionary(x => x.Key, x => x.Value);

            var list = (field.data != null ? field.data.Select(x => new SelectListItem()
            {
                Value = x.IdFieldValue.ToString(),
                Text = x.FieldValue,
                Selected = value == x.IdFieldValue.ToString()
            }) : Enumerable.Empty<SelectListItem>()).ToList();

            if (!field.IsValueRequired) list.Insert(0, new SelectListItem() { Text = "Не выбрано", Value = "", Selected = string.IsNullOrEmpty(value) });

            return html.DropDownList($"fieldValue_{field.IdField}", list, htmlAttributes);
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
