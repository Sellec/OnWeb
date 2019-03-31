using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class HiddenSimpleMultiLineFieldType : FieldType
    {
        public override ValuesValidationResult Validate(IEnumerable<object> values, IField field)
        {
            if (values.Count() > 0 && values.First() != null && !values.First().GetType().IsValueType)
            {
                var validationValue = values.First().ToString();
            }

            return new ValuesValidationResult(values);
        }

        //todo RenderHtmlEditor
        //public override MvcHtmlString RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        //{
        //    if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
        //    htmlAttributes["style"] = "display:none;";
        //    if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

        //    var value = (field as Data.FieldData)?.ToString();
        //    return html.TextArea($"fieldValue_{field.IdField}", value, htmlAttributes);
        //}

        public override int IdType
        {
            get { return 9; } 
        }

        public override string TypeName
        {
            get { return "Скрытое многострочное поле"; }
        }

        public override bool IsRawOrSourceValue
        {
            get { return true; }
        }

        public override bool? ForcedIsMultipleValues
        {
            get { return false; }
        }
    }
}
