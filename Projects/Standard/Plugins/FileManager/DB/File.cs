namespace OnWeb.Modules.FileManager.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo ������ �����������.
    [Table("File")]
    public partial class File
    {
        [Key]
        public int IdFile { get; set; }

        public int IdModule { get; set; }

        [Required]
        public string NameFile { get; set; }

        /// <summary>
        /// ���� � ����� ������������ �������� ����� �����.
        /// </summary>
        [Required]
        public string PathFile { get; set; }

        [StringLength(255)]
        public string UniqueKey { get; set; }

        /// <summary>
        /// ����� ��� �����.
        /// </summary>
        public FileTypeCommon TypeCommon { get; set; }

        /// <summary>
        /// ������ ��� �����, ������������ �� ������ ����������, ���������� � ��.
        /// </summary>
        public string TypeConcrete { get; set; }

        public int DateChange { get; set; }

        public DateTime? DateExpire { get; set; }

        public int IdUserChange { get; set; }
    }
}
