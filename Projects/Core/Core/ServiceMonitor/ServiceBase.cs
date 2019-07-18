using System;
using OnUtils.Application.ServiceMonitor;

namespace OnWeb.Core.ServiceMonitor
{
#pragma warning disable CS1591 // todo внести комментарии.
    public abstract class ServiceBase : ServiceBase<WebApplicationBase>, IMonitoredService
    {
        public ServiceBase()
        {
        }

        public ServiceBase(Guid serviceID) : base(serviceID)
        {
        }

        public ServiceBase(Guid serviceID, string serviceName) : base(serviceID, serviceName)
        {
        }
    }
}