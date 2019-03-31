namespace OnWeb.Core.ModuleExtensions.CustomFields.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("CustomFieldsField")]
    public class CustomFieldsField : Items.ItemBase, Field.IField
    {
        private bool _IsMultipleValues = false;
        private Field.FieldValueType _IdValueType = Field.FieldValueType.Default;

        /// Возвращает или задает идентификатор поля.
        [Key]
        [Column("field_id")]
        public int IdField { get; set; }

        [Column("field_module1")]
        public int IdModule { get; set; }

        [Required]
        [StringLength(200)]
        [Column("field_name")]
        public string name { get; set; }

        [StringLength(100)]
        [Column("NameFieldAlt")]
        public string nameAlt { get; set; }

        [Column("field_type")]
        public int IdFieldType { get; set; }

        /// <summary>
        /// Возвращает тип поля (см. описание <see cref="Field.FieldType"/>) на основании значения <see cref="IdFieldType"/>. 
        /// Если значение <see cref="IdFieldType"/> не соответствует ни одному из известных типов полей (см. <see cref="Field.FieldTypesCollection"/>), то возвращает <see cref="Field.FieldTypesCollection.UnknownField"/>.
        /// </summary>
        [NotMapped]
        public Field.FieldType FieldType { get => IdFieldType; }

        [StringLength(100)]
        [Column("field_alias")]
        public string alias { get; set; }

        [Column("field_size")]
        public int size { get; set; }

        public string field_data { get; set; }

        [Column("field_mustvalue")]
        public bool IsValueRequired { get; set; }

        [Column("IsMultipleValues")]
        public bool IsMultipleValues
        {
            get
            {
                if (FieldType.ForcedIsMultipleValues.HasValue) _IsMultipleValues = FieldType.ForcedIsMultipleValues.Value;
                return _IsMultipleValues;
            }
            set
            {
                _IsMultipleValues = value;
                if (FieldType.ForcedIsMultipleValues.HasValue) _IsMultipleValues = FieldType.ForcedIsMultipleValues.Value;
            }
        }

        public int status { get; set; }

        public int Block { get; set; }

        [StringLength(100)]
        [Column("NameEnding")]
        public string nameEnding { get; set; }

        [StringLength(200)]
        [Column("FormatCheck")]
        public string formatCheck { get; set; }

        public int IdSource { get; set; }

        public float ParameterNumeric01 { get; set; }

        public float ParameterNumeric02 { get; set; }

        public int DateChange { get; set; }

        [Column("IdValueType")]
        public Field.FieldValueType IdValueType
        {
            get
            {
                if (FieldType.ForcedIdValueType.HasValue) _IdValueType = FieldType.ForcedIdValueType.Value;
                return _IdValueType;
            }
            set
            {
                _IdValueType = value;
                if (FieldType.ForcedIdValueType.HasValue) _IdValueType = FieldType.ForcedIdValueType.Value;
            }
        }

        #region Items.ItemBase
        public override int ID
        {
            get { return IdField; }
            set { IdField = value; }
        }

        public override string Caption
        {
            get { return name; }
            set { name = value; }
        }

        public override DateTime DateChangeBase
        {
            get { return DateChange.FromTimestamp(); }
            set { DateChange = value.Timestamp(); }
        }

        #endregion

        [ForeignKey("IdField")]
        public Field.ValueVariantCollection data
        {
            get;
            set;
        }

        #region Методы
        /// <summary>
        /// Возвращает отображаемое название поля. Если задан, то выводится <see cref="nameAlt"/>. Если нет, то выводится <see cref="name"/>.
        /// </summary>
        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(nameAlt) ? name : nameAlt;
        }

        /// <summary>
        /// Возвращает представление для значения поля в зависимости от типа данных поля (в зависимости от <see cref="GetValueType"/>) и типа вывода.
        /// </summary>
        /// <param name="value">Значение, которое следует преобразовать. Тип значения должен соответствовать типу, возвращаемому методом <see cref="GetValueType"/>.</param>
        /// <param name="outputType">Тип вывода. Если равен <see cref="Field.OutputType.Text"/>, то возвращается <see cref="string"/>. Если равен <see cref="Field.OutputType.Html"/>, то возвращается <see cref="MvcHtmlString"/> </param>
        public string GetDisplayValue(object value, Field.OutputType outputType = Field.OutputType.Text)
        {
            var valueType = GetValueType();
            if (valueType == typeof(Boolean) && value is bool)
            {
                return (bool)value ? "Да" : "Нет";
            }
            else if (IdValueType == Field.FieldValueType.URL)
            {
                if (outputType == Field.OutputType.Html) return $"<a href='{value.ToString()}'>Ссылка</a>";
                return "";
            }

            return value.ToString();
        }

        /// <summary>
        /// Возвращает Runtime-тип, соответствующий допустимому типу значений поля.
        /// </summary>
        public Type GetValueType()
        {
            switch (IdValueType)
            {
                case Field.FieldValueType.Default:
                case Field.FieldValueType.String:
                    return typeof(string);

                case Field.FieldValueType.Int:
                    return typeof(long);

                case Field.FieldValueType.Boolean:
                    return typeof(bool);

                case Field.FieldValueType.FloatComma:
                case Field.FieldValueType.FloatDot:
                    return typeof(decimal);

                case Field.FieldValueType.KeyFromSource:
                    return typeof(int);

                case Field.FieldValueType.URL:
                    return typeof(string);
            }

            return typeof(string);
        }

        #endregion
    }
}
