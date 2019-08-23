using OnUtils.Application.Messaging;
using System;

namespace OnWeb.Messaging
{
    /// <summary>
    /// Базовая реализация сервиса отправки-приема сообщений для веб-приложения.
    /// </summary>
    /// <typeparam name="TMessageType">Тип сообщения, с которым работает сервис.</typeparam>
    public abstract class MessageServiceBase<TMessageType> : MessageServiceBase<WebApplication, TMessageType>
        where TMessageType : MessageBase, new()
    {
        /// <summary>
        /// </summary>
        protected MessageServiceBase(string serviceName, Guid serviceID) : base(serviceName, serviceID)
        {
        }
    }
}
