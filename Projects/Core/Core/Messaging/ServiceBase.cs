using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnWeb.Core.Messaging
{
    using ServiceMonitor;

#pragma warning disable CS1591 // todo внести комментарии.
    public abstract class ServiceBase<TMessageType> : CoreComponentBase<ApplicationCore>, IMonitoredService, IMessagingServiceBackgroundOperations
        where TMessageType : MessageBase, new()
    {
        protected ServiceBase(string serviceName, Guid serviceID, int? idMessageType = null)
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            if (serviceID == null) throw new ArgumentNullException(nameof(serviceID));

            ServiceID = serviceID;
            ServiceName = serviceName;

            if (!idMessageType.HasValue) IdMessageType = (byte)Items.ItemTypeFactory.GetItemType(typeof(TMessageType)).IdItemType;
        }

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

        #region Сообщения
        /// <summary>
        /// Регистрирует сообщение <paramref name="message"/> в очередь на отправку.
        /// </summary>
        /// <returns>Возвращает true в случае успеха и false в случае ошибки во время регистрации сообщения. Текст ошибки </returns>
        protected bool RegisterMessage(TMessageType message)
        {
            try
            {
                // todo setError(null);

                using (var db = new DB.CoreContext())
                {
                    var mess = new DB.MessageQueue()
                    {
                        IdMessageType = IdMessageType,
                        IsSent = false,
                        MessageInfo = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message)),
                        DateCreate = DateTime.Now,
                    };

                    db.MessageQueue.Add(mess);
                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                // todo setError("Ошибка во время регистрации сообщения.", ex);
                return false;
            }
        }

        protected Dictionary<DB.MessageQueue, TMessageType> GetUnsentMessages(int IdMessageType)
        {
            try
            {
                // todo setError(null);

                using (var db = new DB.CoreContext())
                {
                    var messages = db.MessageQueue.Where(x => x.IdMessageType == IdMessageType && !x.IsSent).ToList();
                    var messagesUnserialized = messages.ToDictionary(x => x, x =>
                    {
                        var str = Encoding.UTF8.GetString(x.MessageInfo);
                        try
                        {
                            return Newtonsoft.Json.JsonConvert.DeserializeObject<TMessageType>(str);
                        }
                        catch
                        {
                            return null;
                        }
                    });
                    return messagesUnserialized;
                }
            }
            catch (Exception ex)
            {
                // todo setError("Ошибка во время загрузки неотправленных сообщений.", ex);
                return null;
            }
        }
        #endregion

        #region Методы
        public void Init()
        {
            //if (initializationContext == ProviderInitializationContext.Constructor)
            {
                this.RegisterServiceState(ServiceStatus.RunningIdeal, "Сервис запущен.");
            }
        }

        public void Dispose()
        {
            this.RegisterServiceState(ServiceStatus.Shutdown, "Сервис остановлен.");
        }

        protected List<Connectors.IConnectorBase<TMessageType>> GetConnectors()
        {
            return AppCore.Get<IMessagingManager>().GetConnectorsByMessageType<TMessageType>().ToList();
        }

        public virtual int GetOutcomingQueueLength()
        {
            using (var db = new UnitOfWork<DB.MessageQueue>())
            {
                return db.Repo1.Where(x => !x.IsSent && x.IdMessageType == IdMessageType).Count();
            }
        }
        #endregion

        #region Фоновые операции
        void IMessagingServiceBackgroundOperations.ExecuteIncoming()
        {
        }

        void IMessagingServiceBackgroundOperations.ExecuteOutcoming()
        {
            int messagesAll = 0;
            int messagesSent = 0;
            int messagesErrors = 0;

            try
            {
                using (var db = new DB.CoreContext())
                {
                    var messages = GetUnsentMessages(IdMessageType);
                    if (messages == null) throw new Exception("");// todo  getError(), getErrorException());

                    messagesAll = messages.Count;

                    OnBeforeExecuteOutcoming(messagesAll);

                    var time = new MeasureTime();
                    foreach (var message in messages)
                    {
                        if (message.Value == null) continue;

                        MessageProcessed<TMessageType> messageBody = null;

                        foreach (var connector in GetConnectors())
                        {
                            try
                            {
                                messageBody = new MessageProcessed<TMessageType>(message.Value, message.Key);
                                connector.Send(messageBody, this);
                                if (messageBody.IsHandled) break;
                            }
                            catch (Exception ex)
                            {
                                messageBody.IsHandled = true; 
                                messageBody.IsError = true;
                                messageBody.ErrorText = $"Неожиданная ошибка во время обработки сообщения в коннекторе '{connector.ConnectorName}' (обработка прервана): {ex.Message}";
                                break;
                            }
                        }

                        if (messageBody == null) messageBody = new MessageProcessed<TMessageType>(message.Value, message.Key) { IsHandled = true, IsError = true, ErrorText = "Нет ни одного коннектора." };

                        if (messageBody.IsHandled)
                        {
                            message.Key.ExternalID = messageBody.ExternalID;

                            if (messageBody.IsSuccess)
                            {
                                message.Key.IsSent = messageBody.IsSuccess;
                                message.Key.DateSent = DateTime.Now;
                                messagesSent++;
                            }
                            else if (messageBody.IsError) messagesErrors++;

                            db.MessageQueueHistory.Add(new DB.MessageQueueHistory()
                            {
                                IdQueue = message.Key.IdQueue,
                                DateEvent = DateTime.Now,
                                EventText = messageBody.IsError ? "Зафиксированы ошибки при отправке: " + (string.IsNullOrEmpty(messageBody.ErrorText) ? "Неизвестная ошибка во время отправки" : messageBody.ErrorText) : "Сообщение обработано",
                                IsSuccess = messageBody.IsSuccess,
                            });

                            db.MessageQueue.AddOrUpdate(x => x.IdQueue, message.Key);

                        }

                        if (time.Calculate(false).TotalSeconds >= 3)
                        {
                            db.SaveChanges();
                            time.Start();
                        }
                    }

                    db.SaveChanges();
                }

                if (messagesAll > 0)
                {
                    this.RegisterServiceState(messagesErrors == 0 ? ServiceStatus.RunningIdeal : ServiceStatus.RunningWithErrors, $"Сообщений в очереди - {messagesAll}. Отправлено - {messagesSent}. Ошибки отправки - {messagesErrors}.");
                }

                var service = AppCore.Get<Monitor>().GetService(ServiceID);
                if (service != null && (DateTime.Now - service.LastDateEvent).TotalHours >= 1)
                {
                    this.RegisterServiceState(ServiceStatus.RunningIdeal, $"Писем нет, сервис работает без ошибок.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Messaging.Email.ExecuteOutcoming: {0}", ex.GetMessageExtended());
                this.RegisterServiceState(ServiceStatus.RunningWithErrors, $"Сообщений в очереди - {messagesAll}. Отправлено - {messagesSent}. Ошибки отправки - {messagesErrors}.", ex);
            }
        }

        protected virtual void OnBeforeExecuteOutcoming(int messagesCount)
        {

        }

        private void TryToSendThrougnConnectors(MessageProcessed<TMessageType> messageBody)
        {
        }

        #endregion

        #region Свойства
        public virtual byte IdMessageType { get; set; }

        public Guid ServiceID
        {
            get;
            private set;
        }

        public string ServiceName
        {
            get;
            private set;
        }

        #region IMonitoredService
        /// <summary>
        /// См. <see cref="IMonitoredService.ServiceStatus"/>.
        /// </summary>
        public virtual ServiceStatus ServiceStatus
        {
            get;
            protected set;
        }

        /// <summary>
        /// См. <see cref="IMonitoredService.ServiceStatusDetailed"/>.
        /// </summary>
        public virtual string ServiceStatusDetailed
        {
            get;
            protected set;
        }

        /// <summary>
        /// См. <see cref="IMonitoredService.IsSupportsCurrentStatusInfo"/>.
        /// </summary>
        public virtual bool IsSupportsCurrentStatusInfo
        {
            get;
            protected set;
        }
        #endregion

        #region IMessagingServiceBackgroundOperations
        /// <summary>
        /// См. <see cref="IMessagingService.IsSupportsIncoming"/>.
        /// </summary>
        public virtual bool IsSupportsIncoming
        {
            get;
            protected set;
        }

        /// <summary>
        /// См. <see cref="IMessagingService.IsSupportsIncoming"/>.
        /// </summary>
        public virtual bool IsSupportsOutcoming
        {
            get;
            protected set;
        }

        /// <summary>
        /// См. <see cref="IMessagingService.IsSupportsIncoming"/>.
        /// </summary>
        public virtual int OutcomingQueueLength
        {
            get;
            protected set;
        }
        #endregion
        #endregion
    }
}
