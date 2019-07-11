using OnUtils.Architecture.AppCore;
using System.Collections.Generic;
using System.Linq;
#if NETFULL
using System.Web.Mvc;
using System.Web.Mvc.Html;
#elif NETCORE
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MvcHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class SourceMultipleFieldTypeRender : CoreComponentBase<WebApplicationCore>, ICustomFieldRender<SourceMultipleFieldType>
    {
        MvcHtmlString ICustomFieldRender<SourceMultipleFieldType>.RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            var value = (field as Data.FieldData)?.Value;
            var values = (value as IEnumerable<int>)?.Select(x => x);

            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

            htmlAttributes = htmlAttributes.Where(x => x.Key.ToLower() != "size").ToDictionary(x => x.Key, x => x.Value);
            //htmlAttributes["multiple"] = true;

            var list = (field.data != null ? field.data.Select(x => new SelectListItem()
            {
                Value = x.IdFieldValue.ToString(),
                Text = x.FieldValue,
                Selected = values != null && values.Contains(x.IdFieldValue)
            }) : Enumerable.Empty<SelectListItem>()).ToList();

            return html.ListBox($"fieldValue_{field.IdField}[]", list, htmlAttributes);
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
