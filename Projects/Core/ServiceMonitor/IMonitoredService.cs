using OnUtils.Application.ServiceMonitor;

namespace OnWeb.ServiceMonitor
{
    using Core;

    /// <summary>
    /// Представляет сервис, который можно отслеживать через <see cref="Monitor"/>.
    /// </summary>
    public interface IMonitoredService : IMonitoredService<WebApplication>, IComponent, IComponentStartable
    {
    }
}
