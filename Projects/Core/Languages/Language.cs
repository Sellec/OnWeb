namespace OnWeb.Languages
{
    /// <summary>
    /// Описывает языковой пакет.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Идентификатор языкового пакета
        /// </summary>
        public int IdLanguage { get; set; }

        /// <summary>
        /// Название языкового пакета.
        /// </summary>
        public string NameLanguage { get; set; }

        /// <summary>
        /// Короткое обозначение языка в международном стандарте.
        /// todo сделать большой Enum.
        /// </summary>
        public string ShortName { get; set; }
    }
}
