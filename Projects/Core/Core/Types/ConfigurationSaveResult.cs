namespace OnWeb.Types
{
    /// <summary>
    /// Предоставляет варианты результата выполнения функции сохранения настроек.
    /// </summary>
    public enum ConfigurationSaveResult
    {
        /// <summary>
        /// Сохранение прошло успешно.
        /// </summary>
        Success,

        /// <summary>
        /// Доступ к сохранению настроек запрещен.
        /// </summary>
        PermissionsDenied,

        /// <summary>
        /// Неизвестная ошибка.
        /// </summary>
        UnknownError
    }
}
