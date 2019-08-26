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
    public abstract class IncomingMessageReceiver<TMessage> : MessageServiceComponent<TMessage>, IIncomingMessageReceiver<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
        /// <summary>
        /// Создает новый экземпляр компонента.
        /// </summary>
        /// <param name="name">Имя компонента</param>
        /// <param name="usingOrder">Определяет очередность вызова компонента, если существует несколько компонентов, обрабатывающих один вид сообщений.</param>
        protected IncomingMessageReceiver(string name, uint? usingOrder = null) : base(name, usingOrder)
        {
        }

        #region Виртуальные методы
        /// <summary>
        /// Возвращает новые сообщения для регистрации в сервисе для дальнейшей обработки.
        /// </summary>
        /// <param name="service">Сервис обработки сообщений, в котором будут зарегистрированы новые сообщения.</param>
        /// <remarks>Дополнительные типы исключений, которые могут возникнуть во время получения сообщений, могут быть описаны в документации компонента.</remarks>
        [ApiIrreversible]
        protected abstract List<MessageInfo<TMessage>> OnReceive(MessageServiceBase<TMessage> service);
        #endregion

        #region IIncomingMessageReceiver
        List<MessageInfo<TMessage>> IIncomingMessageReceiver<WebApplication, TMessage>.Receive(MessageServiceBase<WebApplication, TMessage> service)
        {
            return OnReceive((MessageServiceBase<TMessage>)service);
        }
        #endregion
    }
}
