namespace OnWeb.Core.Journaling.DB
{
    using Items;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляет запись в журнале.
    /// </summary>
    [Table("Journal1")]
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

        /// <summary>
        /// Идентификатор объекта, с которым связано событие. Связанный объект возможно получить, когда задано значение <see cref="IdRelatedItem"/> и <see cref="IdRelatedItemType"/>.
        /// </summary>
        /// <seealso cref="ItemBase.ID"/>.
        public int? IdRelatedItem { get; set; }

        /// <summary>
        /// Идентификатор типа объекта, с которым связано событие. Связанный объект возможно получить, когда задано значение <see cref="IdRelatedItem"/> и <see cref="IdRelatedItemType"/>.
        /// </summary>
        /// <see cref="ItemType.IdItemType"/>
        public int? IdRelatedItemType { get; set; }

    }
}
