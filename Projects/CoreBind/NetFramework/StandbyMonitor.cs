using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;

namespace OnWeb.Core
{
    using ServiceMonitor;
    using Utils.Factory;
    using Utils.Tasks;

    public class StandbyMonitor : SingletonBase<StandbyMonitor>, IMonitoredService
    {
        private Guid _serviceID = Guid.Empty;

        private ConcurrentDictionary<string, DateTime> _monitoredRootAddressed = new ConcurrentDictionary<string, DateTime>();

        public StandbyMonitor()
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes((this as IMonitoredService).ServiceName));
                _serviceID = new Guid(hash);
            }
        }

        public void Init()
        {
            TasksManager.SetTask(typeof(StandbyMonitor).FullName + "_" + nameof(StandbyMonitor.BackgroundTask) + "_minutely5", Cron.MinuteInterval(5), () => StandbyMonitor.BackgroundTaskStatic());
            this.RegisterServiceState(ServiceStatus.RunningIdeal, "Сервис запущен.");
        }

        //public override void Dispose()
        //{
        //    this.RegisterServiceState(ServiceStatus.Shutdown, "Сервис остановлен.");
        //}

        public void CheckUri(Uri uri)
        {
            _monitoredRootAddressed.TryAdd(uri.ToString(), DateTime.Now);
        }

        public static void BackgroundTaskStatic()
        {
            StandbyMonitor.Instance.BackgroundTask();
        }

        public void BackgroundTask()
        {
            if (Debug.IsDeveloper) return;

            try
            {
                var client = new WebClient();
                client.Encoding = Encoding.UTF8;

                var defaultWebProxy = WebRequest.DefaultWebProxy;
                defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;

                client.Proxy = defaultWebProxy;

                try
                {
                    var answerBytes = client.DownloadData(ApplicationCore.Instance.ServerUrl);
                    this.RegisterServiceState(ServiceStatus.RunningIdeal, $"Проверка адреса сервера прошла успешно.");
                }
                catch (Exception ex)
                {
                    this.RegisterServiceState(ServiceStatus.RunningWithErrors, $"Проверка адреса сервера прошла с ошибкой: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                this.RegisterServiceState(ServiceStatus.RunningWithErrors, $"Error: {ex.Message}", ex);
            }
        }

        #region IMonitoredService
        Guid IMonitoredService.ServiceID
        {
            get => _serviceID;
        }

        string IMonitoredService.ServiceName
        {
            get => "Standby";
        }

        ServiceStatus IMonitoredService.ServiceStatus
        {
            get => throw new NotImplementedException();
        }

        string IMonitoredService.ServiceStatusDetailed
        {
            get => throw new NotImplementedException();
        }

        bool IMonitoredService.IsSupportsCurrentStatusInfo
        {
            get => false;
        }

        #endregion
    }
}