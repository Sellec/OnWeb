namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("UserEntity")]
    public partial class UserEntity
    {
        [Key]
        public int IdEntity { get; set; }

        public int IdUser { get; set; }

        [Required]
        [StringLength(200)]
        public string Tag { get; set; }

        [Required]
        [StringLength(200)]
        public string EntityType { get; set; }

        [Required]
        public string Entity { get; set; }

        [Required]
        public Boolean IsTagged { get; set; }

    }
}
