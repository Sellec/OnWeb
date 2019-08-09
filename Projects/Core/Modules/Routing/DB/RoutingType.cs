namespace OnWeb.Modules.Routing.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("UrlTranslationType")]
    public partial class RoutingType
    {
        #region Отражение базы
        [Key]
        public eTypes IdTranslationType { get; set; }

        [Required]
        [StringLength(100)]
        public string NameTranslationType { get; set; }

        [Required]
        [StringLength(200)]
        public string DescriptionTranslationType { get; set; }

        #endregion

        #region Дополнительно
        public enum eTypes
        {
            Main = 1,
            Additional = 3,
            Old = 2
        }
        #endregion
    }
}
