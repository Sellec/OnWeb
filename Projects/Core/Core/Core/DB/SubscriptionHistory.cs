namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("SubscriptionHistory")]
    public partial class SubscriptionHistory
    {
        public int id { get; set; }

        public int subscr_id { get; set; }

        [Required]
        [StringLength(200)]
        public string email { get; set; }
    }
}
