using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.MessageHandlers;

namespace OnWeb.Messaging.MessageHandlers
{
    /// <summary>
    /// Представляет обработчик сообщений.
    /// </summary>
    public interface IMessageHandler<TMessage> : IMessageHandler<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
