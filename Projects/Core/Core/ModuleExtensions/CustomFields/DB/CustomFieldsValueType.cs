namespace OnWeb.Core.ModuleExtensions.CustomFields.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("CustomFieldsValueType")]
    public partial class CustomFieldsValueType
    {
        [Key]
        public int IdValueType { get; set; }

        [Required]
        [StringLength(100)]
        public string NameValueType { get; set; }
    }
}
