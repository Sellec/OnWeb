using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.Components;
using OnUtils.Application.Messaging.Messages;
using System;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Базовый класс компонента для обработки зарегистрированных входящих сообщений определенного типа.
    /// </summary>
    public abstract class IncomingMessageHandler<TMessage> : IncomingMessageHandler<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
        /// <summary>
        /// Создает новый экземпляр компонента.
        /// </summary>
        protected IncomingMessageHandler()
        {
        }

        /// <summary>
        /// Создает новый экземпляр компонента.
        /// </summary>
        /// <param name="name">Имя компонента</param>
        /// <param name="usingOrder">Определяет очередность вызова компонента, если существует несколько компонентов, обрабатывающих один вид сообщений.</param>
        protected IncomingMessageHandler(string name, uint? usingOrder = null) : base(name, usingOrder)
        {
        }

        /// <summary>
        /// Отправляет указанное сообщение.
        /// </summary>
        /// <param name="messageInfo">Информация о сообщении, которое необходимо отправить</param>
        /// <param name="service">Сервис обработки сообщений, которому принадлежит сообщение <paramref name="messageInfo"/>.</param>
        /// <returns>Если возвращает true, то сообщение считается обработанным (см. <see cref="MessageStateType.Completed"/>).</returns>
        /// <remarks>Дополнительные типы исключений, которые могут возникнуть во время отправки сообщения, могут быть описаны в документации компонента.</remarks>
        [ApiIrreversible]
        protected abstract bool OnPrepare(MessageInfo<TMessage> messageInfo, MessageServiceBase<TMessage> service);

        /// <summary>
        /// </summary>
        protected sealed override bool OnPrepare(MessageInfo<TMessage> messageInfo, MessageServiceBase<WebApplication, TMessage> service)
        {
            return OnPrepare(messageInfo, (MessageServiceBase<TMessage>)service);
        }
    }
}
