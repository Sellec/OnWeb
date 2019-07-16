using OnUtils.Application.ServiceMonitor;

namespace OnWeb.Core.ServiceMonitor
{
    /// <summary>
    /// Представляет сервис, который можно отслеживать через <see cref="Monitor"/>.
    /// </summary>
    public interface IMonitoredService : IMonitoredService<WebApplicationBase>
    {
    }
}
