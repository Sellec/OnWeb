namespace OnWeb.Core.Journaling.DB
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляет заголовок журнала и основную информацию о журнале.
    /// </summary>
    [Table("JournalName")]
    public class JournalName
    {
        /// <summary>
        /// Идентификатор журнала.
        /// </summary>
        [Key]
        public int IdJournal { get; set; }

        /// <summary>
        /// Тип журнала.
        /// </summary>
        public int IdJournalType { get; set; }

        /// <summary>
        /// Название журнала.
        /// </summary>
        [Column("JournalName")]
        public string Name { get; set; }

        /// <summary>
        /// Уникальный ключ журнала
        /// </summary>
        public string UniqueKey { get; set; }
    }
}
