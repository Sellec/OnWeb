namespace DaData.Constants
{
    /// <summary>
    /// Уровень распознавания.
    /// </summary>
    public sealed class QC
    {
        /// <summary>
        /// Исходное значение распознано уверенно. Не требуется ручная проверка
        /// </summary>
        public const string QC_OK = "0";

        /// <summary>
        /// Исходное значение распознано с допущениями или не распознано. Требуется ручная проверка
        /// </summary>
        public const string QC_UNSURE = "1";

        /// <summary>
        /// Исходное значение пустое или заведомо "мусорное"
        /// </summary>
        public const string QC_INVALID = "2";
    }
}
