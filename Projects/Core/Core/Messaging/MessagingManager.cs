using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using OnUtils.Tasks;
using OnUtils.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OnWeb.Core.Messaging
{
    using Connectors;
    using Plugins.MessagingEmail;

    /// <summary>
    /// Предоставляет доступ к сервисам отправки/приема сообщений.
    /// </summary>
    class MessagingManager : CoreComponentBase<ApplicationCore>, IMessagingManager
    {
        class InstanceActivatedHandlerImpl : IInstanceActivatedHandler
        {
            private readonly MessagingManager _manager;

            public InstanceActivatedHandlerImpl(MessagingManager manager)
            {
                _manager = manager;
            }

            void IInstanceActivatedHandler.OnInstanceActivated<TRequestedType>(object instance)
            {
                if (instance is IMessagingService service)
                {
                    if (!_manager._services.Contains(service)) _manager._services.Add(service);
                }
            }
        }

        class MessageFake : MessageBase
        {

        }

        private static MethodInfo _connectorInitCall = null;
        private static ApplicationCore _appCore = null;
        private volatile bool _incomingLock = false;
        private volatile bool _outcomingLock = false;

        private readonly InstanceActivatedHandlerImpl _instanceActivatedHandler = null;
        private List<IMessagingService> _services = new List<IMessagingService>();

        private object _activeConnectorsSyncRoot = new object();
        private List<IComponentTransient<ApplicationCore>> _activeConnectors = null;

        static MessagingManager()
        {
            _connectorInitCall = typeof(IConnectorBase<>).GetMethod(nameof(IConnectorBase<MessageFake>.Init), BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(string) }, null);
            if (_connectorInitCall == null) throw new TypeInitializationException(typeof(MessagingManager).FullName, new Exception($"Ошибка поиска метода '{nameof(IConnectorBase<MessageFake>.Init)}'"));
        }

        public MessagingManager()
        {
            _instanceActivatedHandler = new InstanceActivatedHandlerImpl(this);
        }

        #region CoreComponentBase
        protected sealed override void OnStart()
        {
            _appCore = AppCore;
            AppCore.ObjectProvider.RegisterInstanceActivatedHandler(_instanceActivatedHandler);

            // Попытка инициализировать все сервисы отправки сообщений, наследующиеся от IMessagingService.
            var types = AppCore.GetQueryTypes().Where(x => x.GetInterfaces().Contains(typeof(IMessagingService))).ToList();
            foreach (var type in types)
            {
                try
                {
                    var instance = AppCore.Get<IMessagingService>(type);
                    if (instance != null && !_services.Contains(instance)) _services.Add(instance);
                }
                catch { }
            }

            TasksManager.SetTask(typeof(MessagingManager).FullName + "_" + nameof(PrepareIncoming) + "_minutely1", Cron.MinuteInterval(1), () => PrepareIncomingTasks());
            TasksManager.SetTask(typeof(MessagingManager).FullName + "_" + nameof(PrepareOutcoming) + "_minutely1", Cron.MinuteInterval(1), () => PrepareOutcomingTasks());
        }

        protected sealed override void OnStop()
        {
            TasksManager.RemoveTask(typeof(MessagingManager).FullName + "_" + nameof(PrepareIncoming) + "_minutely1");
            TasksManager.RemoveTask(typeof(MessagingManager).FullName + "_" + nameof(PrepareOutcoming) + "_minutely1");
        }
        #endregion

        #region Методы
        public static void PrepareIncomingTasks()
        {
            if (_appCore != null && _appCore.Get<IMessagingManager>() is MessagingManager manager) manager.PrepareIncoming();
        }

        public void PrepareIncoming()
        {
            if (_incomingLock) return;

            try
            {
                _incomingLock = true;

                foreach (var provider in _services.Where(x => x.IsSupportsIncoming && x is IMessagingServiceBackgroundOperations).Select(x => x as IMessagingServiceBackgroundOperations))
                {
                    try
                    {
                        provider.ExecuteIncoming();
                    }
                    catch (Exception ex) { Debug.WriteLine("Ошибка обработки входящих сообщений для сервиса '{0}': {1}", provider.ServiceName, ex.Message); }
                }

            }
            catch (Exception ex) { Debug.WriteLine("Ошибка обработки входящих сообщений: {0}", ex.Message); }
            finally { _incomingLock = false; }
        }

        public static void PrepareOutcomingTasks()
        {
            if (_appCore != null && _appCore.Get<IMessagingManager>() is MessagingManager manager) manager.PrepareOutcoming();
        }

        public void PrepareOutcoming()
        {
            if (_outcomingLock) return;

            try
            {
                _outcomingLock = true;
                foreach (var provider in _services.Where(x => x.IsSupportsOutcoming && x is IMessagingServiceBackgroundOperations).Select(x => x as IMessagingServiceBackgroundOperations))
                {
                    try
                    {
                        provider.ExecuteOutcoming();
                    }
                    catch (Exception ex) { Debug.WriteLine("Ошибка обработки исходящих сообщений для сервиса '{0}': {1}", provider.ServiceName, ex.GetMessageExtended()); }
                }

            }
            catch (Exception ex) { Debug.WriteLine("Ошибка обработки исходящих сообщений: {0}", ex.GetMessageExtended()); }
            finally { _outcomingLock = false; }
        }
        #endregion

        #region IMessagingManager
        IEnumerable<IConnectorBase<TMessage>> IMessagingManager.GetConnectorsByMessageType<TMessage>()
        {
            lock (_activeConnectorsSyncRoot)
                if (_activeConnectors == null)
                    ((IMessagingManager)this).UpdateConnectorsFromSettings();

            return _activeConnectors.OfType<IConnectorBase<TMessage>>();
        }

        IEnumerable<ICriticalMessagesReceiver> IMessagingManager.GetCriticalMessagesReceivers()
        {
            return _services.OfType<ICriticalMessagesReceiver>();
        }

        IEnumerable<IMessagingService> IMessagingManager.GetMessagingServices()
        {
            return _services.ToList();
        }

        void IMessagingManager.UpdateConnectorsFromSettings()
        {
            lock (_activeConnectorsSyncRoot)
            {
                if (_activeConnectors != null)
                    _activeConnectors.ForEach(x =>
                    {
                        try
                        {
                            x.Stop();
                        }
                        catch (Exception ex)
                        {
                            this.RegisterEvent(Journaling.EventType.Error, "Ошибка при закрытии коннектора", $"Возникла ошибка при выгрузке коннектора типа '{x.GetType().FullName}'.", null, ex);
                        }
                    });

                _activeConnectors = new List<IComponentTransient<ApplicationCore>>();

                var connectorsSettings = AppCore.Config.ConnectorsSettings;
                if (connectorsSettings != null)
                {
                    var types = AppCore.GetQueryTypes().Where(x => TypeHelpers.ExtractGenericInterface(x, typeof(IConnectorBase<>)) != null).ToList();

                    foreach (var setting in connectorsSettings)
                    {
                        var connectorType = types.FirstOrDefault(x => x.FullName == setting.ConnectorTypeName);
                        if (connectorType == null)
                        {
                            this.RegisterEvent(Journaling.EventType.Error, "Ошибка при поиске коннектора", $"Не найден тип коннектора из настроек - '{setting.ConnectorTypeName}'. Для стирания старых настроек следует зайти в настройку коннекторов и сделать сохранение.");
                            continue;
                        }

                        try
                        {
                            var connector = AppCore.Create<IComponentTransient<ApplicationCore>>(connectorType);
                            var initResult = (bool)_connectorInitCall.Invoke(connector, new object[] { setting.SettingsSerialized });
                            if (!initResult)
                            {
                                this.RegisterEvent(Journaling.EventType.Error, "Отказ инициализации коннектора", $"Коннектор типа '{setting.ConnectorTypeName}' ('{connector.GetType().FullName}') вернул отказ инициализации. См. журналы ошибок для поиска возможной информации.");
                                continue;
                            }

                            _activeConnectors.Add(connector);
                        }
                        catch (Exception ex)
                        {
                            this.RegisterEvent(Journaling.EventType.Error, "Ошибка создания коннектора", $"Во время создания и инициализации коннектора типа '{setting.ConnectorTypeName}' возникла неожиданная ошибка.", null, ex);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
