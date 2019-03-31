using System;

namespace OnWeb.Core.ServiceMonitor
{
#pragma warning disable CS1591 // todo внести комментарии.
    public class ServiceInfo
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public ServiceStatus LastStatus { get; set; }

        public string LastStatusDetailed { get; set; }

        public DateTime LastDateEvent { get; set; }
    }
}
