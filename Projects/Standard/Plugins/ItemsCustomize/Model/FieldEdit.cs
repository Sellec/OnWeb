using OnUtils.Application.Modules.ItemsCustomize.DB;
using OnUtils.Application.Modules.ItemsCustomize.Field;
using System;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Plugins.ItemsCustomize.Model
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class FieldEdit
    {
        public FieldEdit()
        {
        }

        public FieldEdit(CustomFieldsField source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            IdModule = source.IdModule;
            IdField = source.IdField;
            name = source.name;
            nameAlt = source.nameAlt;
            alias = source.alias;
            formatCheck = source.formatCheck;
            IdFieldType = source.IdFieldType;
            size = source.size;
            IsValueRequired = source.IsValueRequired;
            IdValueType = source.IdValueType;
            field_data = source.field_data;
            status = source.status;
            Block = source.Block;
            nameEnding = source.nameEnding;
            data = source.data;
        }

        public int IdModule { get; set; }

        public int IdField { get; set; }

        [Required]
        [StringLength(200)]
        public string name { get; set; }

        [StringLength(100)]
        public string nameAlt { get; set; }

        [StringLength(100)]
        public string alias { get; set; }

        [StringLength(200)]
        public string formatCheck { get; set; }

        public int IdFieldType { get; set; }

        public int size { get; set; }

        public bool IsValueRequired { get; set; }

        public FieldValueType IdValueType { get; set; }

        public string field_data { get; set; }

        public int status { get; set; }

        public int Block { get; set; }

        [StringLength(100)]
        public string nameEnding { get; set; }

        public ValueVariantCollection data
        {
            get;
            set;
        }
    }
}
