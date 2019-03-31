using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.ObjectPool;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Описывает сервис отправки/приема сообщений.
    /// </summary>
    public interface IMessagingService : IPoolObject, ServiceMonitor.IMonitoredService, IComponentSingleton<ApplicationCore>
    {
        /// <summary>
        /// Указывает, что сервис поддерживает прием сообщений.
        /// </summary>
        bool IsSupportsIncoming { get; }

        /// <summary>
        /// Указывает, что сервис поддерживает отправку сообщений.
        /// </summary>
        bool IsSupportsOutcoming { get; }

        /// <summary>
        /// Возвращает длину очереди на отправку сообщений.
        /// </summary>
        int GetOutcomingQueueLength();
    }
}
