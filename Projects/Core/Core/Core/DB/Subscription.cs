namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("Subscription")]
    public partial class Subscription
    {
        public int id { get; set; }

        [Required]
        [StringLength(200)]
        public string name { get; set; }

        [Required]
        public string description { get; set; }

        public short status { get; set; }

        public byte AllowSubscribe { get; set; }

        [StringLength(100)]
        public string UniqueKey { get; set; }
    }
}
