namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("SubscriptionEmail")]
    public partial class SubscriptionEmail
    {
        public int id { get; set; }

        public int subscr_id { get; set; }

        [Required]
        [StringLength(200)]
        public string email { get; set; }

        [Required]
        [StringLength(200)]
        public string password { get; set; }

        public int IdUserChange { get; set; }

        public int DateChange { get; set; }
    }
}
