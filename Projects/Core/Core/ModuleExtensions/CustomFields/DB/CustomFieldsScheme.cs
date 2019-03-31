namespace OnWeb.Core.ModuleExtensions.CustomFields.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("CustomFieldsScheme")]
    public partial class CustomFieldsScheme
    {
        [Key]
        public int IdScheme { get; set; }

        public int IdModule { get; set; }

        [Required]
        [StringLength(200)]
        public string NameScheme { get; set; }
    }
}
