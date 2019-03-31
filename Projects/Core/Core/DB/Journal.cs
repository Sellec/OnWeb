namespace OnWeb.Core.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляет запись в журнале.
    /// </summary>
    [Table("Journal")]
    public class Journal
    {
        /// <summary>
        /// Идентификатор записи журнала.
        /// </summary>
        [Key]
        public int IdJournalData { get; set; }

        /// <summary>
        /// Идентификатор журнала.
        /// </summary>
        public int IdJournal { get; set; }

        /// <summary>
        /// Тип события.
        /// </summary>
        public Journaling.EventType EventType { get; set; }

        /// <summary>
        /// Основная информация о событии.
        /// </summary>
        [Required, MaxLength(300)]
        public string EventInfo { get; set; }

        /// <summary>
        /// Детализированная информация о событии.
        /// </summary>
        public string EventInfoDetailed { get; set; }

        /// <summary>
        /// Информация об исключении, если событие сопровождалось возникновением исключения.
        /// </summary>
        public string ExceptionDetailed { get; set; }

        /// <summary>
        /// Дата фиксации события.
        /// </summary>
        public DateTime DateEvent { get; set; }
    }
}
