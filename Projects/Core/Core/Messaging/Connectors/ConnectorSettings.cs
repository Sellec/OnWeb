using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Messaging.Connectors
{
    /// <summary>
    /// Хранит настройки определенного коннектора.
    /// </summary>
    public class ConnectorSettings
    {
        /// <summary>
        /// Полное имя типа коннектора. Используется в <see cref="IMessagingManager.GetConnectorsByMessageType{TMessage}"/> для поиска настроек коннекторов.
        /// </summary>
        public string ConnectorTypeName { get; set; }

        /// <summary>
        /// Сериализованные в строку настройки коннектора. Способ сериализации зависит от коннектора.
        /// </summary>
        public string SettingsSerialized { get; set; }
    }
}
