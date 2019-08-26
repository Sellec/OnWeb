using OnUtils.Application.Messaging.Components;
using OnUtils.Application.Messaging.Messages;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Представляет компонент для обработки зарегистрированных входящих сообщений определенного типа.
    /// </summary>
    public interface IIncomingMessageHandler<TMessage> : IMessageServiceComponent<TMessage>, IIncomingMessageHandler<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
