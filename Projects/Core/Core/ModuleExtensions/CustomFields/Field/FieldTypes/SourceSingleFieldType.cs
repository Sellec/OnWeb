using System.Collections.Generic;
using System.Linq;

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

        //internal override bool ConvertTo(IField field, FieldValueProviderResult result, Type type, CultureInfo culture, out object valueConverted)
        //{
        //    valueConverted = null;
        //    return (result.RawFromForm == null || result.RawFromForm.Length == 0 || result.RawFromForm.Where(x => string.IsNullOrEmpty(x)).Count() == result.RawFromForm.Length);
        //}

        public override int IdType
        {
            get => 2; 
        }

        public override string TypeName
        {
            get => "Группа элементов 'Radiobutton'";
        }

        public override bool IsRawOrSourceValue
        {
            get => false;
        }

        public override bool? ForcedIsMultipleValues
        {
            get => false;
        }

        public override FieldValueType? ForcedIdValueType
        {
            get => FieldValueType.KeyFromSource;
        }
    }
}
