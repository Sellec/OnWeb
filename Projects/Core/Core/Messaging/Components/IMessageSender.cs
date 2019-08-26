using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.Components;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Представляет обработчик для отправки сообщений определенного типа.
    /// </summary>
    public interface IMessageSender<TMessage> : IMessageHandler<TMessage>, IMessageSender<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
