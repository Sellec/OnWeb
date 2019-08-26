using OnUtils.Application.Messaging.Components;
using OnUtils.Application.Messaging.Messages;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Представляет компонент для получения и регистрации сообщений определенного типа.
    /// </summary>
    public interface IIncomingMessageReceiver<TMessage> : IMessageServiceComponent<TMessage>, IIncomingMessageReceiver<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
