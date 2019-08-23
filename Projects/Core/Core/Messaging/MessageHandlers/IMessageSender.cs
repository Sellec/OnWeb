using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.MessageHandlers;

namespace OnWeb.Messaging.MessageHandlers
{
    /// <summary>
    /// Представляет обработчик для отправки сообщений определенного типа.
    /// </summary>
    public interface IMessageSender<TMessage> : IMessageHandler<TMessage>, IMessageSender<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
