namespace OnWeb.Modules.FileManager.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("File")]
    public partial class File
    {
        [Key]
        public int IdFile { get; set; }

        public int IdModule { get; set; }

        [Required]
        public string NameFile { get; set; }

        /// <summary>
        /// Путь к файлу относительно корневой папки сайта.
        /// </summary>
        [Required]
        public string PathFile { get; set; }

        [StringLength(255)]
        public string UniqueKey { get; set; }

        /// <summary>
        /// Общий тип файла.
        /// </summary>
        public FileTypeCommon TypeCommon { get; set; }

        /// <summary>
        /// Точный тип файла, определенный на основе расширения, заголовков и пр.
        /// </summary>
        public string TypeConcrete { get; set; }

        public int DateChange { get; set; }

        /// <summary>
        /// Дата истечения времени жизни файла, после чего файл помечается на удаление и удаляется.
        /// </summary>
        public DateTime? DateExpire { get; set; }

        public int IdUserChange { get; set; }

        /// <summary>
        /// Признак, означающий, что файл помещен в очередь на удаление.
        /// </summary>
        public bool IsRemoving { get; set; }

        /// <summary>
        /// Признак, означающий, что файл удален.
        /// </summary>
        public bool IsRemoved { get; set; }
    }
}
