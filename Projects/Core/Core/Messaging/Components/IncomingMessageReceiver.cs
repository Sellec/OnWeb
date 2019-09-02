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
        /// Вызывается для получения одного сообщения для регистрации в сервисе для дальнейшей обработки. 
        /// После регистрации сообщения вызывается <see cref="OnEndReceive(bool, MessageInfo{TMessage}, MessageServiceBase{TMessage})"/> с информацией об успешности регистрации сообщения.
        /// </summary>
        /// <param name="service">Сервис обработки сообщений, в котором будут зарегистрированы новые сообщения.</param>
        /// <returns>Возвращает информацию о полученном сообщении. Если возвращает не null, то после обработки возвращенного объекта вызывается повторно для получения следующего сообщения до тех пор, пока не вернет null.</returns>
        /// <remarks>
        /// Дополнительные типы исключений, которые могут возникнуть во время получения сообщений, могут быть описаны в документации компонента.
        /// Метод предусматривает обработку ошибок, которые могут возникнуть во время фиксации полученных сообщений. 
        /// Например, если сообщения во внешней системе были помечены как полученные во время вызова метода и после этого возникла ошибка уровня базы данных при полученных сохранении сообщений, 
        /// возможно отменить отметку "Получено" во внешней системе во время вызова <see cref="OnEndReceive(bool, MessageInfo{TMessage}, MessageServiceBase{TMessage})"/>.
        /// </remarks>
        /// <seealso cref="OnEndReceive(bool, MessageInfo{TMessage}, MessageServiceBase{TMessage})"/>
        [ApiIrreversible]
        protected abstract MessageInfo<TMessage> OnBeginReceive(MessageServiceBase<TMessage> service);

        /// <summary>
        /// Вызывается после получения сообщения в <see cref="OnBeginReceive(MessageServiceBase{TMessage})"/> и регистрации в сервисе для дальнейшей обработки. 
        /// </summary>
        /// <param name="service">Сервис обработки сообщений, в котором будут зарегистрированы новые сообщения.</param>
        /// <param name="isSuccess">Признак успешной регистрации в сервисе.</param>
        /// <param name="messageInfo">Информация о сообщении.</param>
        /// <returns>Если <paramref name="isSuccess"/> равен false, то возвращаемый результат ни на что не влияет. Если <paramref name="isSuccess"/> равен true, то в случае возврата false зарегистрированное сообщение удаляется.</returns>
        /// <remarks>
        /// Дополнительные типы исключений, которые могут возникнуть во время получения сообщений, могут быть описаны в документации компонента.
        /// Метод предусматривает обработку ошибок, которые могут возникнуть во время установки отметки "Получено" во внешней системе. 
        /// Например, если <paramref name="isSuccess"/> равен true (т.е. сообщение было успешно зарегистрировано) и не удалось поставить отметку о получении во внешней системе (то есть сообщение будет повторно получено в <see cref="OnBeginReceive(MessageServiceBase{TMessage})"/>),
        /// то можно вернуть false и зарегистрированное сообщение будет удалено.
        /// </remarks>
        [ApiIrreversible]
        protected abstract bool OnEndReceive(bool isSuccess, MessageInfo<TMessage> messageInfo, MessageServiceBase<TMessage> service);

        #region Base
        /// <summary>
        /// </summary>
        protected sealed override List<MessageInfo<TMessage>> OnReceive(MessageServiceBase<WebApplication, TMessage> service)
        {
            return OnReceive((MessageServiceBase<TMessage>)service);
        }

        /// <summary>
        /// </summary>
        protected sealed override MessageInfo<TMessage> OnBeginReceive(MessageServiceBase<WebApplication, TMessage> service)
        {
            return OnBeginReceive((MessageServiceBase<TMessage>)service);
        }

        /// <summary>
        /// </summary>
        protected sealed override bool OnEndReceive(bool isSuccess, MessageInfo<TMessage> messageInfo, MessageServiceBase<WebApplication, TMessage> service)
        {
            return OnEndReceive(isSuccess, messageInfo, (MessageServiceBase<TMessage>)service);
        }
        #endregion
    }
}
