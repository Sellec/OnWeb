namespace OnWeb.Core.ModuleExtensions.CustomFields.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("CustomFieldsSchemeData")]
    public partial class CustomFieldsSchemeData
    {
        [Key]
        public int IdData { get; set; }

        public int IdModule { get; set; }

        public int IdItemType { get; set; }

        public int IdScheme { get; set; }

        public int IdSchemeItem { get; set; }

        public int IdField { get; set; }

        public int Order { get; set; }
    }
}
