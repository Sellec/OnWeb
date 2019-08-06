using OnUtils.Application.Messaging;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Описывает сервис отправки/приема сообщений для веб-приложения.
    /// </summary>
    public interface IMessagingService : IMessagingService<WebApplication>, IComponent, IComponentStartable
    {
    }
}
