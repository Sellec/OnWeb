using System;

namespace OnWeb.Core.ServiceMonitor
{
    /// <summary>
    /// Представляет сервис, который можно отслеживать через <see cref="Monitor"/>.
    /// </summary>
    public interface IMonitoredService : IComponent
    {
        /// <summary>
        /// Возвращает уникальный идентификатор сервиса.
        /// </summary>
        Guid ServiceID { get; }

        /// <summary>
        /// Название сервиса.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Возвращает текущее состояние сервиса.
        /// </summary>
        ServiceStatus ServiceStatus { get; }

        /// <summary>
        /// Возвращает детализированное состояние сервиса.
        /// </summary>
        string ServiceStatusDetailed { get; }

        /// <summary>
        /// Указывает, поддерживает ли сервис получение информации о текущем состоянии.
        /// </summary>
        bool IsSupportsCurrentStatusInfo { get; }
    }
}
