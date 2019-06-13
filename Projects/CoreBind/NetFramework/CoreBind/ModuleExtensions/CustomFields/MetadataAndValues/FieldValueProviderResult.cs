using System;
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

        public override object ConvertTo(Type type, CultureInfo culture)
        {
            object value = null;
            if (_field != null)
            {
                // todo if (_field.FieldType.ConvertTo(_field, this, type, culture, out value))
                //{
                //    return value;
                //}
            }

            var ret = base.ConvertTo(type, culture);
            return ret;
        }

        public string[] RawFromForm { get; private set; }
    }
}
