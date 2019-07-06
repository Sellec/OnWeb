using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace OnWeb.Core.Messaging
{
    using Items;
    using ServiceMonitor;

#pragma warning disable CS1591 // todo внести комментарии.
    public abstract class ServiceBase<TMessageType> : CoreComponentBase<ApplicationCore>, IMonitoredService, IMessagingServiceBackgroundOperations, IUnitOfWorkAccessor<UnitOfWork<DB.MessageQueue, DB.MessageQueueHistory>>
        where TMessageType : MessageBase, new()
    {
        protected ServiceBase(string serviceName, Guid serviceID, int? idMessageType = null)
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));
            if (serviceID == null) throw new ArgumentNullException(nameof(serviceID));

            ServiceID = serviceID;
            ServiceName = serviceName;

            if (!idMessageType.HasValue) IdMessageType = Items.ItemTypeFactory.GetItemType(typeof(TMessageType)).IdItemType;
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
        [ApiReversible]
        protected bool RegisterMessage(TMessageType message)
        {
            try
            {
                // todo setError(null);

                using (var db = this.CreateUnitOfWork())
                {
                    var mess = new DB.MessageQueue()
                    {
                        IdMessageType = IdMessageType,
                        StateType = MessageStateType.NotProcessed,
                        DateCreate = DateTime.Now,
                        MessageInfo = Newtonsoft.Json.JsonConvert.SerializeObject(message),
                    };

                    db.Repo1.Add(mess);
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

        private List<IntermediateStateMessage<TMessageType>> GetUnsentMessages(UnitOfWork<DB.MessageQueue, DB.MessageQueueHistory> db)
        {
            var messages = db.Repo1.
                Where(x => x.IdMessageType == IdMessageType && (x.StateType == MessageStateType.NotProcessed || x.StateType == MessageStateType.RepeatWithControllerType)).
                ToList();

            var messagesUnserialized = messages.Select(x =>
            {
                try
                {
                    var str = x.MessageInfo;
                    return new IntermediateStateMessage<TMessageType>(Newtonsoft.Json.JsonConvert.DeserializeObject<TMessageType>(str), x);
                }
                catch (Exception ex)
                {
                    return new IntermediateStateMessage<TMessageType>(null, x) { StateType = MessageStateType.Error, State = ex.Message, DateChange = DateTime.Now };
                }
            }).ToList();

            return messagesUnserialized;
        }
        #endregion

        #region Методы
        public void Init()
        {
            this.RegisterServiceState(ServiceStatus.RunningIdeal, "Сервис запущен.");
        }

        public void Dispose()
        {
            this.RegisterServiceState(ServiceStatus.Shutdown, "Сервис остановлен.");
        }

        protected List<Connectors.IConnectorBase<TMessageType>> GetConnectors()
        {
            return AppCore.Get<MessagingManager>().GetConnectorsByMessageType<TMessageType>().ToList();
        }

        [ApiReversible]
        public virtual int GetOutcomingQueueLength()
        {
            using (var db = new UnitOfWork<DB.MessageQueue>())
            {
                return db.Repo1.Where(x => x.IdMessageType == IdMessageType && (x.StateType == MessageStateType.NotProcessed || x.StateType == MessageStateType.RepeatWithControllerType)).Count();
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
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress)) // Здесь Suppress вместо RequiresNew, т.к. весь процесс отправки занимает много времени и блокировать таблицу нельзя.
                {
                    var messages = GetUnsentMessages(db);
                    if (messages == null) return;

                    messagesAll = messages.Count;

                    OnBeforeExecuteOutcoming(messagesAll);

                    var processedMessages = new List<IntermediateStateMessage<TMessageType>>();

                    var time = new MeasureTime();
                    foreach (var intermediateMessage in messages)
                    {
                        if (intermediateMessage.StateType == MessageStateType.Error)
                        {
                            processedMessages.Add(intermediateMessage);
                            continue;
                        }

                        var connectors = GetConnectors().Select(x => new { Connector = x, IdTypeConnector = ItemTypeFactory.GetItemType(x.GetType())?.IdItemType }).OrderBy(x => x.Connector.OrderInPool).ToList();
                        if (intermediateMessage.IdTypeConnector.HasValue)
                            connectors = connectors.Where(x => x.IdTypeConnector.HasValue && x.IdTypeConnector == intermediateMessage.IdTypeConnector).ToList();

                        foreach (var connectorInfo in connectors)
                        {
                            try
                            {
                                var connector = connectorInfo.Connector;
                                var connectorMessage = new ConnectorMessage<TMessageType>(intermediateMessage);
                                connector.Send(connectorMessage, this);
                                if (connectorMessage.HandledState != ConnectorMessageStateType.NotHandled)
                                {
                                    intermediateMessage.DateChange = DateTime.Now;
                                    switch (connectorMessage.HandledState)
                                    {
                                        case ConnectorMessageStateType.Error:
                                            intermediateMessage.StateType = MessageStateType.Error;
                                            intermediateMessage.State = connectorMessage.State;
                                            intermediateMessage.IdTypeConnector = null;
                                            break;

                                        case ConnectorMessageStateType.RepeatWithControllerType:
                                            intermediateMessage.StateType = MessageStateType.RepeatWithControllerType;
                                            intermediateMessage.State = connectorMessage.State;
                                            intermediateMessage.IdTypeConnector = connectorInfo.IdTypeConnector;
                                            break;

                                        case ConnectorMessageStateType.Sent:
                                            intermediateMessage.StateType = MessageStateType.Sent;
                                            intermediateMessage.State = null;
                                            intermediateMessage.IdTypeConnector = null;
                                            break;
                                    }
                                    processedMessages.Add(intermediateMessage);
                                    break;
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }

                    if (processedMessages.Count > 0)
                    {
                        db.SaveChanges();

                        //if (messageBody != null && messageBody.IsHandled)
                        //{
                        //    messageSource.Key.ExternalID = messageBody.ExternalID;
                        //    messageSource.Key.IsHandled = true;
                        //    messageSource.Key.IsSent = messageBody.IsSuccess;

                        //    if (messageBody.IsSuccess)
                        //    {
                        //        messageSource.Key.DateSent = DateTime.Now;
                        //        messagesSent++;
                        //    }
                        //    else if (messageBody.IsError) messagesErrors++;

                        //    db.MessageQueueHistory.Add(new DB.MessageQueueHistory()
                        //    {
                        //        IdQueue = messageSource.Key.IdQueue,
                        //        DateEvent = DateTime.Now,
                        //        EventText = messageBody.IsError ? "Зафиксированы ошибки при отправке: " + (string.IsNullOrEmpty(messageBody.ErrorText) ? "Неизвестная ошибка во время отправки" : messageBody.ErrorText) : "Сообщение обработано",
                        //        IsSuccess = messageBody.IsSuccess,
                        //    });

                        //    db.MessageQueue.AddOrUpdate(x => x.IdQueue, messageSource.Key);

                        //}

                        if (time.Calculate(false).TotalSeconds >= 3)
                        {
                            db.SaveChanges();
                            time.Start();
                        }
                    }

                    db.SaveChanges();
                    scope.Commit();
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

        private void TryToSendThrougnConnectors(IntermediateStateMessage<TMessageType> messageBody)
        {
        }

        #endregion

        #region Свойства
        public virtual int IdMessageType { get; set; }

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
