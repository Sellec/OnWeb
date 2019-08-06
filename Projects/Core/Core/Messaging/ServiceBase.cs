using OnUtils.Application.Messaging;
using System;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Предпочтительная базовая реализация сервиса отправки-приема сообщений для веб-приложения.
    /// </summary>
    /// <typeparam name="TMessageType">Тип сообщения, с которым работает сервис.</typeparam>
    public abstract class ServiceBase<TMessageType> : ServiceBase<WebApplication, TMessageType>
        where TMessageType : MessageBase, new()
    {
        protected ServiceBase(string serviceName, Guid serviceID, int? idMessageType = null) : base(serviceName, serviceID, idMessageType)
        {
        }
    }
}
