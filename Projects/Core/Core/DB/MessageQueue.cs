namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("MessageQueue")]
    public partial class MessageQueue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdQueue { get; set; }

        public byte IdMessageType  { get; set; }

        public byte[] MessageInfo { get; set; }

        public DateTime DateCreate { get; set; }

        public bool IsSent { get; set; }

        public DateTime? DateSent { get; set; }

        [MaxLength(100)]
        public string ExternalID { get; set; }

    }
}
