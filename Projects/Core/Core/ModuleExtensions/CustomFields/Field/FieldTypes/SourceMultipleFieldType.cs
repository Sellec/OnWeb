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
