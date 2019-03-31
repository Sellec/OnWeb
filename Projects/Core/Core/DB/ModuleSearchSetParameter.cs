namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("ModuleSearchSetParameter")]
    public partial class ModuleSearchSetParameter
    {
        [Key]
        public int IdSearchSetParameter { get; set; }

        public int IdSearchSet { get; set; }

        [Required]
        [StringLength(50)]
        public string NameParameter { get; set; }

        [StringLength(196)]
        public string ValueParameter { get; set; }
    }
}
