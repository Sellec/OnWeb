﻿using OnUtils.Application.Modules.ItemsCustomize;
using OnUtils.Application.Modules.ItemsCustomize.Field;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Modules.ItemsCustomize.MetadataAndValues
{
    class FieldValueProvider : IValueProvider
    {
        private Core.Modules.ModuleControllerBase _controllerContext;

        public FieldValueProvider(ControllerContext controllerContext)
        {
            _controllerContext = controllerContext.Controller as Core.Modules.ModuleControllerBase;
        }

        private bool GetValue(string prefix, out string[] valueOut, out IField fieldOut)
        {
            valueOut = null;
            fieldOut = null;

            var prefixParts = prefix.Split('.');
            if (prefixParts.Length == 0) return false;

            if (prefixParts.Last() == nameof(IItemCustomized.Fields)) return true;

            if (prefixParts.Length >= 2 &&
                prefixParts[prefixParts.Length - 2] == nameof(IItemCustomized.Fields) &&
                prefixParts.Last().StartsWith("fieldValue_"))
            {
                int idField = 0;
                if (!int.TryParse(prefixParts.Last().Replace("fieldValue_", ""), out idField)) return false;

                var field = _controllerContext.AppCore.Get<ModuleItemsCustomize<WebApplication>>().GetFieldByID(idField);
                if (field == null) return false;

                fieldOut = field;

                var key = _controllerContext.HttpContext.Request.Form.AllKeys.Where(x => x.Replace("[]", "").EndsWith(prefixParts.Last())).FirstOrDefault();
                if (string.IsNullOrEmpty(key)) return false;

                var value = _controllerContext.HttpContext.Request.Form.GetValues(key);
                //if (value.Where(x => !string.IsNullOrEmpty(x)).Count() > 0) // Убрал проверку на непустые значения, чтобы можно было использовать значение "Не выбрано".
                if (value.Count() == 0) return false;

                valueOut = value;
                return true;
            }
            else return false;
        }

        public bool ContainsPrefix(string prefix)
        {
            string[] value;
            IField field;
            return GetValue(prefix, out value, out field);
        }

        public ValueProviderResult GetValue(string key)
        {
            string[] value;
            IField field;
            if (!GetValue(key, out value, out field)) return null;
            if (field == null) return null; // Для GetValue if (prefixParts.Last() == nameof(Web.Items.ItemBase.Fields)) не должно возвращать значение.

            return new FieldValueProviderResult(field, value, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}