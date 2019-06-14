﻿using OnUtils.Architecture.AppCore;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class SimpleSingleLineFieldTypeRender : CoreComponentBase<ApplicationCore>, ICustomFieldRender<SimpleSingleLineFieldType>
    {
        MvcHtmlString ICustomFieldRender<SimpleSingleLineFieldType>.RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        {
            var value = (field as Data.FieldData)?.ToString();
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

            return html.TextBox($"fieldValue_{field.IdField}", value, htmlAttributes);
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