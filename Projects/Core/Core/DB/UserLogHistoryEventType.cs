namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("UserLogHistoryEventType")]
    public partial class UserLogHistoryEventType
    {
        [Key]
        public int IdEventType { get; set; }

        [Required]
        [StringLength(200)]
        public string NameEventType { get; set; }
    }
}
