namespace OnWeb.Modules.FileManager.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("FileRemoveQueue")]
    partial class FileRemoveQueue
    {
        [Key]
        public int IdFile { get; set; }
    }
}
