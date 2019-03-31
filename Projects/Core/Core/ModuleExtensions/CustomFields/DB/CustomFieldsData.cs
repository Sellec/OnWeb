namespace OnWeb.Core.ModuleExtensions.CustomFields.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("CustomFieldsData")]
    public class CustomFieldsData
    {
        [Key]
        public int IdFieldData { get; set; }

        public int IdField { get; set; }

        public int IdItem { get; set; }

        public int IdItemType { get; set; }

        public int IdFieldValue { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string FieldValue { get; set; }

        public int DateChange { get; set; }
    }

}
