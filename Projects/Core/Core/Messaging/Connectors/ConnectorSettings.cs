namespace OnWeb.Core.Messaging.Connectors
{
    /// <summary>
    /// Хранит настройки определенного коннектора.
    /// </summary>
    public class ConnectorSettings
    {
        /// <summary>
        /// Полное имя типа коннектора. Используется в <see cref="MessagingManager.GetConnectorsByMessageType{TMessage}"/> для поиска настроек коннекторов.
        /// </summary>
        public string ConnectorTypeName { get; set; }

        /// <summary>
        /// Сериализованные в строку настройки коннектора. Способ сериализации зависит от коннектора.
        /// </summary>
        public string SettingsSerialized { get; set; }
    }
}
