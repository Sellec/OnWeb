using OnUtils.Application.Journaling;
using OnUtils.Application.Journaling.DB;
using OnUtils.Application.ServiceMonitor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OnWeb.Core.ServiceMonitor
{
    using Journaling;

    /// <summary>
    /// Монитор сервисов. Позволяет вносить и получать информацию о сервисах, о их состоянии, событиях и пр.
    /// </summary>
    public sealed class Monitor : CoreComponentBase, IComponentSingleton
    {
        private static ConcurrentDictionary<Guid, JournalName> _servicesJournalsList = new ConcurrentDictionary<Guid, JournalName>();
        private static ConcurrentDictionary<Guid, ServiceInfo> _servicesList = new ConcurrentDictionary<Guid, ServiceInfo>();

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion

        /// <summary>
        /// Фиксирует состояние сервиса без записи в журнал.
        /// </summary>
        /// <param name="service">Сервис, для которого производится регистрация состояния.</param>
        /// <param name="serviceStatus">Состояние сервиса.</param>
        /// <param name="serviceStatusDetailed">Детализированное состояние сервиса.</param>
        public void RegisterServiceStateWithoutJournal(IMonitoredService service, ServiceStatus serviceStatus, string serviceStatusDetailed = null)
        {
            AppCore.Get<Monitor<WebApplication>>().RegisterServiceStateWithoutJournal(service, serviceStatus, serviceStatusDetailed);
        }

        /// <summary>
        /// Фиксирует состояние сервиса.
        /// </summary>
        /// <param name="service">Сервис, для которого производится регистрация состояния.</param>
        /// <param name="serviceStatus">Состояние сервиса.</param>
        /// <param name="serviceStatusDetailed">Детализированное состояние сервиса.</param>
        /// <param name="exception">Ошибки, если были зарегистрированы.</param>
        public void RegisterServiceState(IMonitoredService service, ServiceStatus serviceStatus, string serviceStatusDetailed = null, Exception exception = null)
        {
            AppCore.Get<Monitor<WebApplication>>().RegisterServiceState(service, serviceStatus, serviceStatusDetailed, exception);
        }

        /// <summary>
        /// Записывает в журнал сервиса событие, связанное с сервисом.
        /// </summary>
        /// <param name="service">Сервис, для которого производится регистрация состояния.</param>
        /// <param name="eventType">См. <see cref="JournalingManager.RegisterEvent"/>.</param>
        /// <param name="eventInfo">См. <see cref="JournalingManager.RegisterEvent"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="JournalingManager.RegisterEvent"/>.</param>
        /// <param name="exception">См. <see cref="JournalingManager.RegisterEvent"/>.</param>
        public void RegisterServiceEvent(IMonitoredService service, EventType eventType, string eventInfo, string eventInfoDetailed = null, Exception exception = null)
        {
            AppCore.Get<Monitor<WebApplication>>().RegisterServiceEvent(service, eventType, eventInfo, eventInfoDetailed, exception);
        }

        /// <summary>
        /// Возвращает журнал для указанного сервиса.
        /// </summary>
        public IEnumerable<Journal> GetServiceJournal(IMonitoredService service)
        {
            return AppCore.Get<Monitor<WebApplication>>().GetServiceJournal(service);
        }

        /// <summary>
        /// Возвращает журнал для указанного идентификатора сервиса.
        /// </summary>
        public IEnumerable<Journal> GetServiceJournal(Guid serviceID)
        {
            return AppCore.Get<Monitor<WebApplication>>().GetServiceJournal(serviceID);
        }

        /// <summary>
        /// Возвращает список сервисов.
        /// </summary>
        public IDictionary<Guid, ServiceInfo> GetServicesList()
        {
            return AppCore.Get<Monitor<WebApplication>>().GetServicesList();
        }

        /// <summary>
        /// Возвращает сервис с указанным идентификатором.
        /// </summary>
        /// <returns>Объект с данными сервиса или null, если сервис не найден.</returns>
        public ServiceInfo GetService(Guid serviceID)
        {
            return AppCore.Get<Monitor<WebApplication>>().GetService(serviceID);
        }
    }
}

namespace System
{
    using OnUtils.Application.ServiceMonitor;
    using OnWeb.Core.Journaling;
    using OnWeb.Core.ServiceMonitor;

    /// <summary>
    /// </summary>
    public static class MonitorExtension
    {
        /// <summary>
        /// Фиксирует состояние сервиса на момент вызова метода.
        /// </summary>
        /// <param name="service">Сервис, для которого производится регистрация состояния.</param>
        /// <param name="serviceStatus">Состояние сервиса.</param>
        /// <param name="serviceStatusDetailed">Детализированное состояние сервиса.</param>
        /// <param name="exception">Ошибки, если были зарегистрированы.</param>
        public static void RegisterServiceState(this IMonitoredService service, ServiceStatus serviceStatus, string serviceStatusDetailed = null, Exception exception = null)
        {
            service.GetAppCore().Get<Monitor>()?.RegisterServiceState(service, serviceStatus, serviceStatusDetailed, exception);
        }

        /// <summary>
        /// Записывает в журнал сервиса событие, связанное с сервисом.
        /// </summary>
        /// <param name="service">Сервис, для которого производится регистрация состояния.</param>
        /// <param name="eventType">См. <see cref="JournalingManager.RegisterEvent"/>.</param>
        /// <param name="eventInfo">См. <see cref="JournalingManager.RegisterEvent"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="JournalingManager.RegisterEvent"/>.</param>
        /// <param name="exception">См. <see cref="JournalingManager.RegisterEvent"/>.</param>
        public static void RegisterServiceEvent(this IMonitoredService service, EventType eventType, string eventInfo, string eventInfoDetailed = null, Exception exception = null)
        {
            service.GetAppCore().Get<Monitor>()?.RegisterServiceEvent(service, eventType, eventInfo, eventInfoDetailed, exception);
        }

    }
}