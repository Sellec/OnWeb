using OnUtils.Application.Messaging.Components;
using OnUtils.Application.Messaging.Messages;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Представляет компонент сервиса обработки сообщений определенного типа.
    /// </summary>
    public interface IMessageServiceComponent<TMessage> : IMessageServiceComponent<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
    }
}
