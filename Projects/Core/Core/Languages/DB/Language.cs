namespace OnWeb.Languages.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("Language")]
    public partial class Language
    {
        [Key]
        public int IdLanguage { get; set; }

        [Required]
        [StringLength(200)]
        public string NameLanguage { get; set; }

        [Required]
        [StringLength(20)]
        public string ShortAlias { get; set; }

        public int IsDefault { get; set; }

        [Required]
        [StringLength(200)]
        public string TemplatesPath { get; set; }

        [Required]
        [StringLength(20)]
        public string Culture { get; set; }

    }
}
