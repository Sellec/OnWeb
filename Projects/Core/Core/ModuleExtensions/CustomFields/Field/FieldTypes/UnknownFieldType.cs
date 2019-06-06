using System.Collections.Generic;

namespace OnWeb.Core.ModuleExtensions.CustomFields.Field.FieldTypes
{
    sealed class UnknownFieldType : FieldType
    {
        public override ValuesValidationResult Validate(IEnumerable<object> values, IField field)
        {
            return new ValuesValidationResult("Неизвестное поле.");
        }

        //todo RenderHtmlEditor
        //public override MvcHtmlString RenderHtmlEditor<TModel>(HtmlHelper<TModel> html, IField field, IDictionary<string, object> htmlAttributes, params object[] additionalParameters)
        //{
        //    return MvcHtmlString.Empty;
        //}

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
