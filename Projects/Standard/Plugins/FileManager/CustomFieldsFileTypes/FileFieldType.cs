using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;

namespace OnWeb.Plugins.FileManager.CustomFieldsFileTypes
{
    using Core.ModuleExtensions.CustomFields.Field;
    using DB;

    public class FileFieldType : FieldType
    {
        class c : RequiredAttribute
        {
            public string GetErrorMessageString()
            {
                return ErrorMessageString;
            }
        }

        [ThreadStatic]
        private static UnitOfWork<File> DB;

        private static UnitOfWork<File> GetDB()
        {
            if (DB == null) DB = new UnitOfWork<File>();
            return DB;
        }

        public override ValuesValidationResult Validate(IEnumerable<object> values, IField field)
        {
            if (field.IsValueRequired && (values == null || values.Count() == 0)) return CreateResultForEmptyValue(field);

            var sourceValues = field.data;
            var unknownValues = values.Where(x => sourceValues.Where(y => y.IdFieldValue == (int)x).Count() == 0);

            var valuesPrepared = new HashSet<int>();
            var valuesInvalid = new System.Collections.ObjectModel.Collection<string>();
            foreach (var value in values)
            {
                if (value is int) valuesPrepared.Add((int)value);
                else if (value is File) valuesPrepared.Add((value as File).IdFile);
                else valuesInvalid.Add(value?.ToString()?.Truncate(0, 10, "..."));
            }

            var filesFound = DataAccessManager.Get<File>().Where(x => valuesPrepared.Contains(x.IdFile)).Select(x => x.IdFile).ToList();
            if (field.IsValueRequired && filesFound.Count == 0) return CreateResultForEmptyValue(field);

            var filesUnknown = valuesPrepared.Where(x => !filesFound.Contains(x)).ToList();

            if (filesUnknown.Count > 0 || valuesInvalid.Count > 0)
            {
                var errors = new List<string>();
                if (valuesInvalid.Count > 0) errors.Add("Следующие значения некорректны:\r\n - " + string.Join(";\r\n - ", valuesInvalid) + ".");
                if (filesUnknown.Count > 0) errors.Add("Следующие файлы не найдены: №" + string.Join(", №", filesUnknown) + ".");

                return new ValuesValidationResult(string.Join("\r\n", errors));
            }

            return new ValuesValidationResult(filesFound.Select(x => (object)x));
        }

        protected virtual ValuesValidationResult CreateResultForEmptyValue(IField field)
        {
            var requiredAttribute = new c();
            return new ValuesValidationResult(string.Format(requiredAttribute.GetErrorMessageString(), field.GetDisplayName()));
        }

        public override IEnumerable<CustomAttributeBuilder> GetCustomAttributeBuildersForModel(IField field)
        {
            if (field.IsValueRequired)
            {
                if (field.IsMultipleValues)
                {
                    var requiredAttribute = typeof(RequiredAttributeForMultipleValue).GetConstructor(Type.EmptyTypes);
                    return new CustomAttributeBuilder(requiredAttribute, new object[] { }).ToEnumerable();
                }
                else
                {
                    var requiredAttribute = typeof(RequiredAttributeForSingleValue).GetConstructor(Type.EmptyTypes);
                    return new CustomAttributeBuilder(requiredAttribute, new object[] { }).ToEnumerable();
                }
            }
            return null;
        }

        public override int IdType
        {
            get { return 10; }
        }

        public override string TypeName
        {
            get { return "Файл"; }
        }

        public override bool IsRawOrSourceValue
        {
            get { return false; }
        }

        public override FieldValueType? ForcedIdValueType
        {
            get { return FieldValueType.KeyFromSource; }
        }
    }


}
