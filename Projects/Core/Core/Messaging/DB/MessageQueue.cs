namespace OnWeb.Core.Messaging.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("MessageQueue")]
    partial class MessageQueue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdQueue { get; set; }

        public int IdMessageType  { get; set; }

        public DateTime DateCreate { get; set; }

        public MessageStateType StateType { get; set; }

        public string State { get; set; }

        public int? IdTypeConnector { get; set; }

        public DateTime? DateChange { get; set; }

        public byte[] MessageInfo { get; set; }
    }
}
