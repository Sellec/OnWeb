using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class SourceMultipleFieldType : FieldType
    {
        public override ValuesValidationResult Validate(IEnumerable<object> values, IField field)
        {
            var sourceValues = field.data;
            var unknownValues = values.Where(x => sourceValues.Where(y => y.IdFieldValue == (int)x).Count() == 0);
            if (unknownValues.Count() == 1)
                return new ValuesValidationResult($"Значение '{unknownValues.First()}' не найдено в списке допустимых.");
            else if (unknownValues.Count() > 1)
                return new ValuesValidationResult("Значения " + string.Join(", ", unknownValues) + " не найдены в списке допустимых.");

            return new ValuesValidationResult(values);
        }

        //todo RenderHtmlEditor
        //public override MvcHtmlString RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        //{
        //    var value = (field as Data.FieldData)?.Value;
        //    var values = (value as IEnumerable<int>)?.Select(x => x);

        //    if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
        //    if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

        //    htmlAttributes = htmlAttributes.Where(x => x.Key.ToLower() != "size").ToDictionary(x => x.Key, x => x.Value);
        //    //htmlAttributes["multiple"] = true;

        //    var list = (field.data != null ? field.data.Select(x => new SelectListItem()
        //    {
        //        Value = x.IdFieldValue.ToString(),
        //        Text = x.FieldValue,
        //        Selected = values != null && values.Contains(x.IdFieldValue)
        //    }) : Enumerable.Empty<SelectListItem>()).ToList();

        //    return html.ListBox($"fieldValue_{field.IdField}[]", list, htmlAttributes);
        //}

        //public override MvcHtmlString RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        //{
        //    var value = (field as Data.FieldData)?.Value;
        //    var values = (value as IEnumerable<int>)?.Select(x => x);

        //    if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();
        //    if (!string.IsNullOrEmpty(field.alias)) htmlAttributes["class"] = (htmlAttributes.GetValueOrDefault("class", null) ?? "") + " FieldAlias_" + field.alias;

        //    var builder = new TagBuilder("input");

        //    foreach (var pair in htmlAttributes)
        //        builder.MergeAttribute(pair.Key, pair.Value?.ToString(), true);

        //    builder.MergeAttribute("type", "checkbox", true);
        //    builder.MergeAttribute("name", $"fieldValue_{field.id}[]", true);
        //    builder.MergeAttribute("type", "checkbox", true);

        //    var str = String.Empty;
        //    if (field.data != null)
        //        foreach (var fieldValue in field.data)
        //        {
        //            var _for = $"fieldValue_{field.id}_{fieldValue.IdFieldValue}";

        //            builder.MergeAttribute("id", _for, true);
        //            builder.MergeAttribute("value", fieldValue.IdFieldValue.ToString(), true);

        //            if (values != null && values.Contains(fieldValue.IdFieldValue))
        //                builder.MergeAttribute("checked", null, true);
        //            else
        //                builder.Attributes.Remove("checked");

        //            str += builder.ToString(TagRenderMode.SelfClosing);

        //            str += html.Label(_for, fieldValue.FieldValue).ToString();
        //            str += "<br />";
        //        }

        //    return MvcHtmlString.Create(str);
        //}

        public override int IdType
        {
            get => 3; 
        }

        public override string TypeName
        {
            get => "Группа элементов 'Checkbox'";
        }

        public override bool IsRawOrSourceValue
        {
            get => false;
        }

        public override bool? ForcedIsMultipleValues
        {
            get => true;
        }

        public override FieldValueType? ForcedIdValueType
        {
            get => FieldValueType.KeyFromSource;
        }
    }
}
