namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS1591 // todo внести комментарии.
    [Table("UserLogHistory")]
    public partial class UserLogHistory
    {
        #region Отражение БД
        [Key]
        public int IdRecord { get; set; }

        public int IdUser { get; set; }

        public int DateEvent { get; set; }

        public int IdEventType { get; set; }

        [Required]
        [StringLength(50)]
        public string IP { get; set; }

        public string Comment { get; set; }
        #endregion

        #region Дополнительные свойства
        [ForeignKey("IdUser")]
        public virtual User User { get; set; }

        [ForeignKey("IdEventType")]
        public virtual UserLogHistoryEventType EventType { get; set; }
        #endregion
    }
}
