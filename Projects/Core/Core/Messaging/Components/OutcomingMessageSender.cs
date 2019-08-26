using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.Components;
using OnUtils.Application.Messaging.Messages;
using System;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Базовый класс компонента для отправки сообщений определенного типа.
    /// </summary>
    public abstract class OutcomingMessageSender<TMessage> : MessageServiceComponent<TMessage>, IOutcomingMessageSender<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
        /// <summary>
        /// Создает новый экземпляр компонента.
        /// </summary>
        /// <param name="name">Имя компонента</param>
        /// <param name="usingOrder">Определяет очередность вызова компонента, если существует несколько компонентов, обрабатывающих один вид сообщений.</param>
        protected OutcomingMessageSender(string name, uint? usingOrder = null) : base(name, usingOrder)
        {
        }

        #region Виртуальные методы
        /// <summary>
        /// Отправляет указанное сообщение.
        /// </summary>
        /// <param name="message">Информация о сообщении, которое необходимо отправить</param>
        /// <param name="service">Сервис обработки сообщений, которому принадлежит сообщение <paramref name="message"/>.</param>
        /// <returns>Если возвращает true, то сообщение считается обработанным (см. <see cref="MessageStateType.Completed"/>).</returns>
        /// <remarks>Дополнительные типы исключений, которые могут возникнуть во время отправки сообщения, могут быть описаны в документации компонента.</remarks>
        [ApiIrreversible]
        protected abstract bool OnSend(MessageInfo<TMessage> message, MessageServiceBase<TMessage> service);
        #endregion

        #region IOutcomingMessageSender
        bool IOutcomingMessageSender<WebApplication, TMessage>.Send(MessageInfo<TMessage> message, MessageServiceBase<WebApplication, TMessage> service)
        {
            return OnSend(message, (MessageServiceBase<TMessage>)service);
        }
        #endregion
    }
}
