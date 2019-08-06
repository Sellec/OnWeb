using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.Connectors;

namespace OnWeb.Core.Messaging.Connectors
{
    /// <summary>
    /// Представляет коннектор к сервису отправки или получения сообщений.
    /// </summary>
    public interface IConnectorBase<TMessage> : IConnectorBase<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
