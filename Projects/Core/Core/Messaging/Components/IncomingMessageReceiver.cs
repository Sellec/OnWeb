using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.Components;
using OnUtils.Application.Messaging.Messages;
using System;
using System.Collections.Generic;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Базовый класс компонента для получения и регистрации сообщений определенного типа.
    /// </summary>
    public abstract class IncomingMessageReceiver<TMessage> : IncomingMessageReceiver<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
        /// <summary>
        /// Создает новый экземпляр компонента.
        /// </summary>
        protected IncomingMessageReceiver()
        {
        }

        /// <summary>
        /// Создает новый экземпляр компонента.
        /// </summary>
        /// <param name="name">Имя компонента</param>
        /// <param name="usingOrder">Определяет очередность вызова компонента, если существует несколько компонентов, обрабатывающих один вид сообщений.</param>
        protected IncomingMessageReceiver(string name, uint? usingOrder = null) : base(name, usingOrder)
        {
        }

        /// <summary>
        /// Возвращает новые сообщения для регистрации в сервисе для дальнейшей обработки.
        /// </summary>
        /// <param name="service">Сервис обработки сообщений, в котором будут зарегистрированы новые сообщения.</param>
        /// <remarks>Дополнительные типы исключений, которые могут возникнуть во время получения сообщений, могут быть описаны в документации компонента.</remarks>
        [ApiIrreversible]
        protected abstract List<MessageInfo<TMessage>> OnReceive(MessageServiceBase<TMessage> service);

        /// <summary>
        /// </summary>
        protected sealed override List<MessageInfo<TMessage>> OnReceive(MessageServiceBase<WebApplication, TMessage> service)
        {
            return OnReceive((MessageServiceBase<TMessage>)service);
        }
    }
}
