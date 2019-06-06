using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class SimpleSingleLineFieldType : FieldType
    {
        public override ValuesValidationResult Validate(IEnumerable<object> values, IField field)
        {
            if (values.Count() > 0 && values.First() != null && !values.First().GetType().IsValueType)
            {
                var validationValue = values.First().ToString();
                if (validationValue.Contains('\r') || validationValue.Contains('\n'))
                    return new ValuesValidationResult("Поле не может содержать символ переноса строки или символ переноса каретки.");
            }

            return new ValuesValidationResult(values);
        }

        //public override MvcHtmlString RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        //{
        //    var value = (field as Data.FieldData)?.ToString();
        //    if (htmlAttributes == null || htmlAttributes.IsReadOnly) htmlAttributes = new Dictionary<string, object>(htmlAttributes);
        //    if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

        //    switch (field.IdValueType)
        //    {
        //        case FieldValueType.Email:
        //            htmlAttributes["type"] = "email";
        //            break;

        //        case FieldValueType.Phone:
        //            htmlAttributes["type"] = "phone";
        //            break;

        //        case FieldValueType.URL:
        //            htmlAttributes["type"] = "URL";
        //            break;

        //        case FieldValueType.Boolean:
        //            return html.CheckBox($"fieldValue_{field.IdField}", value == true.ToString(), htmlAttributes);

        //    }

        //    return html.TextBox($"fieldValue_{field.IdField}", value, htmlAttributes);
        //}

        public override int IdType
        {
            get => 1; 
        }

        public override string TypeName
        {
            get => "Простое однострочное поле";
        }

        public override bool IsRawOrSourceValue
        {
            get => true;
        }

        public override bool? ForcedIsMultipleValues
        {
            get => false;
        }
    }
}
