using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.ObjectPool;

namespace OnWeb.Core.Messaging.Connectors
{
    /// <summary>
    /// Представляет коннектор к сервису отправки или получения сообщений.
    /// </summary>
    public interface IConnectorBase<TMessage> : IPoolObjectOrdered, IComponentTransient<ApplicationCore> where TMessage : MessageBase, new()
    {
        #region Методы
        /// <summary>
        /// Инициализация коннектора. При инициализации в качестве аргумента передается строка <paramref name="connectorSettings"/> с сериализованными настройками коннектора.
        /// </summary>
        /// <remarks>Дополнительные типы исключений, которые могут возникнуть во время инициализации, могут быть описаны в документации коннектора.</remarks>
        bool Init(string connectorSettings);

        /// <summary>
        /// Отправляет указанное сообщение.
        /// </summary>
        /// <param name="message">Информация о сообщении, которое необходимо отправить</param>
        /// <param name="service">Сервис отправки сообщений, которому принадлежит отправляемое сообщение <paramref name="message"/>.</param>
        /// <remarks>Дополнительные типы исключений, которые могут возникнуть во время отправки сообщения, могут быть описаны в документации коннектора.</remarks>
        void Send(MessageProcessed<TMessage> message, IMessagingService service);
        #endregion

        #region Свойства
        /// <summary>
        /// Возвращает название коннектора.
        /// </summary>
        string ConnectorName { get; }
        #endregion
    }
}
