using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Core.ModuleExtensions.CustomFields.MetadataAndValues
{
    class FieldValueProviderResult : ValueProviderResult
    {
        private Field.IField _field = null;

        public FieldValueProviderResult(Field.IField field, string[] rawValue, CultureInfo culture) : base(rawValue, rawValue?.FirstOrDefault(), culture)
        {
            RawFromForm = rawValue;
            _field = field;
        }

        public string[] RawFromForm { get; private set; }
    }
}
