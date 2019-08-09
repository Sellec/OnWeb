using OnUtils.Application.Modules.ItemsCustomize.Field;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Modules.ItemsCustomize.MetadataAndValues
{
    class FieldValueProviderResult : ValueProviderResult
    {
        private IField _field = null;

        public FieldValueProviderResult(IField field, string[] rawValue, CultureInfo culture) : base(rawValue, rawValue?.FirstOrDefault(), culture)
        {
            RawFromForm = rawValue;
            _field = field;
        }

        public string[] RawFromForm { get; private set; }
    }
}
