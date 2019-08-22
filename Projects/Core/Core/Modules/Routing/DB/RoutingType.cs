namespace OnWeb.Modules.Routing.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo ������ �����������.
    [Table("UrlTranslationType")]
    public partial class RoutingType
    {
        #region ��������� ����
        [Key]
        public eTypes IdTranslationType { get; set; }

        [Required]
        [StringLength(100)]
        public string NameTranslationType { get; set; }

        [Required]
        [StringLength(200)]
        public string DescriptionTranslationType { get; set; }

        #endregion

        #region �������������
        public enum eTypes
        {
            Main = 1,
            Additional = 3,
            Old = 2
        }
        #endregion
    }
}
