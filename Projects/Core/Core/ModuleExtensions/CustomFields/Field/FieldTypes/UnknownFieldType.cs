using System.Collections.Generic;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
#pragma warning disable CS1591 // todo внести комментарии.
    public sealed class UnknownFieldType : FieldType
    {
        public override ValuesValidationResult Validate(IEnumerable<object> values, IField field)
        {
            return new ValuesValidationResult("Неизвестное поле.");
        }

        public override int IdType
        {
            get => 0; 
        }

        public override string TypeName
        {
            get => "Неизвестный тип поля";
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
