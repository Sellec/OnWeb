﻿using OnUtils.Application.ServiceMonitor;
using System;

namespace OnWeb.ServiceMonitor
{
#pragma warning disable CS1591 // todo внести комментарии.
    public abstract class ServiceBase : ServiceBase<WebApplication>, IMonitoredService
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