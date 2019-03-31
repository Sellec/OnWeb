namespace OnWeb.Core.ModuleExtensions.CustomFields.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("CustomFieldsValue")]
    public class CustomFieldsValue
    {
        [Key]
        public int IdFieldValue { get; set; }

        public int IdField { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(200)]
        public string FieldValue { get; set; }

        public int Order { get; set; }

        public int DateChange { get; set; }

        public int old_index { get; set; }
    }
}
