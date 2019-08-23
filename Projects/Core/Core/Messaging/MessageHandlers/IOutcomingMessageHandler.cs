using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.MessageHandlers;

namespace OnWeb.Messaging.MessageHandlers
{
    /// <summary>
    /// Представляет обработчик для отправки сообщений определенного типа.
    /// </summary>
    public interface IOutcomingMessageHandler<TMessage> : IMessageHandler<TMessage>, IOutcomingMessageHandler<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
