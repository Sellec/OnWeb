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
