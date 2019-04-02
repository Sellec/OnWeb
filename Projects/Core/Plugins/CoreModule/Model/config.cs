namespace OnWeb.Plugins.ModuleCore.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("config")]
    partial class config
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdConfig { get; set; }

        [StringLength(100)]
        public string name { get; set; }

        [Required]
        public string serialized { get; set; }

        public int DateChange { get; set; }

        public int IdUserChange { get; set; }
    }
}
