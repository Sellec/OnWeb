using System.Collections.Generic;
using System.Linq;

// todo using TraceWeb.ModuleExtensions.CustomFields.MetadataAndValues;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class SourceSingleFieldType : FieldType
    {
        public override ValuesValidationResult Validate(IEnumerable<object> values, IField field)
        {
            if (field.IsValueRequired && values.Count() == 0) return new ValuesValidationResult("Значение не может быть пустым.");

            var sourceValues = field.data;
            var unknownValues = values.Where(x => sourceValues.Where(y => y.IdFieldValue == (int)x).Count() == 0);
            if (unknownValues.Count() == 1)
                return new ValuesValidationResult($"Значение '{unknownValues.First()}' не найдено в списке допустимых.");
            else if (unknownValues.Count() > 1)
                return new ValuesValidationResult("Значения " + string.Join(", ", unknownValues) + " не найдены в списке допустимых.");

            return new ValuesValidationResult(values);
        }

        //todo RenderHtmlEditor
        //internal override bool ConvertTo(IField field, FieldValueProviderResult result, Type type, CultureInfo culture, out object valueConverted)
        //{
        //    valueConverted = null;
        //    return (result.RawFromForm == null || result.RawFromForm.Length == 0 || result.RawFromForm.Where(x => string.IsNullOrEmpty(x)).Count() == result.RawFromForm.Length);
        //}

        //public override MvcHtmlString RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        //{
        //    var value = (field as Data.FieldData)?.Value?.ToString();

        //    if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
        //    if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

        //    htmlAttributes = htmlAttributes.Where(x => x.Key.ToLower() != "size").ToDictionary(x => x.Key, x => x.Value);

        //    var list = (field.data != null ? field.data.Select(x => new SelectListItem()
        //    {
        //        Value = x.IdFieldValue.ToString(),
        //        Text = x.FieldValue,
        //        Selected = value == x.IdFieldValue.ToString()
        //    }) : Enumerable.Empty<SelectListItem>()).ToList();

        //    if (!field.IsValueRequired) list.Insert(0, new SelectListItem() { Text = "Не выбрано", Value = "", Selected = string.IsNullOrEmpty(value) });

        //    return html.DropDownList($"fieldValue_{field.IdField}", list, htmlAttributes);
        //}

        public override int IdType
        {
            get { return 2; } 
        }

        public override string TypeName
        {
            get { return "Группа элементов 'Radiobutton'"; }
        }

        public override bool IsRawOrSourceValue
        {
            get { return false; }
        }

        public override bool? ForcedIsMultipleValues
        {
            get { return false; }
        }

        public override FieldValueType? ForcedIdValueType
        {
            get { return FieldValueType.KeyFromSource; }
        }
    }
}
