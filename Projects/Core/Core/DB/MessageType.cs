namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("MessageType")]
    public partial class MessageType
    {
        [Key]
        public byte IdMessageType { get; set; }

        public string NameMessageType  { get; set; }
    }
}
