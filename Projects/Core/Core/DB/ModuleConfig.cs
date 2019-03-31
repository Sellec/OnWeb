namespace OnWeb.Core.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ��������� ��������� ������.
    /// </summary>
    [Table("ModuleConfig")]
    public partial class ModuleConfig
    {
        /// <summary>
        /// ������������� ������.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdModule { get; set; }

        /// <summary>
        /// ���������� ��������, ����������� ���������������� query-��� ������. ������������ ������ ��� query-����.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string UniqueKey { get; set; }

        /// <summary>
        /// ��������������� � json ��������� ������������ ������. ��. <see cref="Configuration.ModuleConfiguration{TModule}"/>.
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// ���� ���������� ��������� ������ � ����.
        /// </summary>
        public DateTime DateChange { get; set; }

        /// <summary>
        /// ������������� ������������, ��������� ��������� � ��������� ���.
        /// </summary>
        public int IdUserChange { get; set; }

    }
}
