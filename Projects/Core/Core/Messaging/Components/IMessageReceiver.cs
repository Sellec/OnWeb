using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.MessageHandlers;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Представляет обработчик для получения и регистрации сообщений определенного типа.
    /// </summary>
    public interface IMessageReceiver<TMessage> : IMessageHandler<TMessage>, IMessageReceiver<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
