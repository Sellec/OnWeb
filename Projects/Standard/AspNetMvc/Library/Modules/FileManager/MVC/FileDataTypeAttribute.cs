using System.Collections.Generic;
using System.Linq;

using OnUtils.Data;
using OnWeb.Modules.FileManager.DB;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Обозначает свойство или поле, хранящее идентификатор загруженного файла, а также предоставляющее встроенный редактор для загрузки файлов
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FileDataTypeAttribute  : DataTypeAttribute
    {
        public FileDataTypeAttribute(FileType fileType = FileType.File) : base("FileUpload")
        {
            FileType = fileType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var values = new List<int>();
            var isMultiple = false;
            var type = value.GetType();

            var interfaceType = OnUtils.Types.TypeHelpers.ExtractGenericInterface(type, typeof(IEnumerable<>));
            if (type.IsArray)
            {
                type = type.GetElementType();
                isMultiple = true;
                if (type == typeof(int) && value != null) { values.AddRange((int[])value); }
            }
            else if (interfaceType != null)
            {
                type = interfaceType.GenericTypeArguments[0];
                isMultiple = true;
                if (type == typeof(int) && value != null) { values.AddRange((IEnumerable<int>)value); }
            }
            else if (type == typeof(int) && value != null) { values.Add((int)value); }
            else if (type == typeof(int?) && value != null) { values.Add((int)value); }

            var allowed = type == typeof(int) || (!isMultiple && type == typeof(int?));
            if (!allowed) return new ValidationResult($"Для файлового поля '{validationContext.DisplayName}' допустим тип данных int?, int, int[] или IEnumerable<int> или его производные.");
            else return CheckFiles(values, validationContext);
        }

        private ValidationResult CheckFiles(IEnumerable<int> files, ValidationContext validationContext)
        {
            if (files != null && files.Count() > 0)
            {
                var filesGrouped = files.GroupBy(x => x).Select(x => x.Key).Where(x => x != 0);

                using (var db = new UnitOfWork<File>())
                {
                    var filesFromDB = db.Repo1.Where(x => filesGrouped.Contains(x.IdFile)).ToDictionary(x => x.IdFile, x => x);

                    var filesNotFound = filesGrouped.Where(x => !filesFromDB.ContainsKey(x)).ToList();
                    if (filesNotFound.Count > 0) return new ValidationResult($"Для поля '{validationContext.DisplayName}' следующие файлы не найдены: {string.Join(", ", filesNotFound)}");

                    //todo проверка на тип файла
                    //var filesFound = filesFromDB
                    //if (filesFound.Count > 0) return new ValidationResult($"Для поля '{validationContext.DisplayName}' следующие файлы не найдены: {string.Join(", ", filesNotFound)}");

                }
            }
            return ValidationResult.Success;
        }

        public FileType FileType
        {
            get;
            private set;
        }
    }
}