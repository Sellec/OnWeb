using OnUtils.Application.Messaging.Components;
using OnUtils.Application.Messaging.Messages;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Представляет компонент для отправки сообщений определенного типа.
    /// </summary>
    public interface IOutcomingMessageSender<TMessage> : IMessageServiceComponent<TMessage>, IOutcomingMessageSender<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
