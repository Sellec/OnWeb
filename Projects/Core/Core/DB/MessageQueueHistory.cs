namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("MessageQueueHistory")]
    public partial class MessageQueueHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdQueueHistory { get; set; }

        public int IdQueue{ get; set; }

        public DateTime DateEvent { get; set; }

        public string EventText { get; set; }

        public bool IsSuccess { get; set; }
    }
}
