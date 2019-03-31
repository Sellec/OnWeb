using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.ObjectPool;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Plugins.Communication.SMS.Connectors
{
    using Core;
    using Core.Messaging;
    using Core.Messaging.Connectors;
    using Core.Messaging.SMS;
    using Journaling = Core.Journaling;
    using Routing = Core.Routing;

    class AmazonSNS : CoreComponentBase<ApplicationCore>, IConnectorBase<Message>, IDisposable
    {
        private AmazonSimpleNotificationServiceClient _clientSNS = null;
        private AmazonCloudWatchLogsClient _clientCloudWatchLogs = null;
        private ConcurrentDictionary<string, string> _options = new ConcurrentDictionary<string, string>();

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
            if (_clientSNS != null)
            {
                _clientSNS.Dispose();
                _clientSNS = null;
            }
            if (_clientCloudWatchLogs != null)
            {
                _clientCloudWatchLogs.Dispose();
                _clientCloudWatchLogs = null;
            }
        }
        #endregion

        #region IConnectorBase<Message>
        bool IConnectorBase<Message>.Init(string connectorSettings)
        {
            var clientCloudWatchLogs = new AmazonCloudWatchLogsClient(
                "",
                "",
                Amazon.RegionEndpoint.USWest2
            );

            _clientCloudWatchLogs = clientCloudWatchLogs;

            var clientSNS = new AmazonSimpleNotificationServiceClient(
                "",
                "",
                Amazon.RegionEndpoint.USWest2
            );

            _clientSNS = clientSNS;

            return true;
        }

        public void Dispose()
        {

        }

        public void Send(MessageProcessed<Message> message, IMessagingService service)
        {
            try
            {
                if (string.IsNullOrEmpty(message.ExternalID))
                {
                     var smsAttributes = new Dictionary<string, MessageAttributeValue>();

                    MessageAttributeValue senderID = new MessageAttributeValue();
                    senderID.DataType = "String";
                    senderID.StringValue = Routing.UrlManager.Translate(AppCore.Config.SiteShortName)?.Truncate(0, 11);
                    if (string.IsNullOrEmpty(senderID.StringValue)) senderID.StringValue = "Site";

                    MessageAttributeValue sMSType = new MessageAttributeValue();
                    sMSType.DataType = "String";
                    sMSType.StringValue = "Transactional";

                    smsAttributes.Add("AWS.SNS.SMS.SenderID", senderID);
                    smsAttributes.Add("AWS.SNS.SMS.SMSType", sMSType);

                    var responseTask = getClientSNS().PublishAsync(new PublishRequest()
                    {
                        Message = message.Message.Body?.ToString(),
                        PhoneNumber = message.Message.To.ContactData,
                        MessageAttributes = smsAttributes
                    });
                    responseTask.Wait();
                    var response = responseTask.Result;

                    message.IsHandled = true;
                    message.ExternalID = response.MessageId;
                }
                else
                {
                    var logGroupName = _options.GetOrAddWithExpiration("LogGroupName", (k) =>
                    {
                        try
                        {
                            var preferencesTask = getClientSNS().GetSMSAttributesAsync(new GetSMSAttributesRequest() { Attributes = new List<string>() { "DeliveryStatusIAMRole" } });
                            preferencesTask.Wait();
                            var preferences = preferencesTask.Result;
                            if (preferences != null && preferences.Attributes != null && preferences.Attributes.Count > 0)
                            {
                                var val = preferences.Attributes["DeliveryStatusIAMRole"];
                                var roleNameMatch = System.Text.RegularExpressions.Regex.Match(preferences.Attributes["DeliveryStatusIAMRole"], "arn:aws:iam::([^:]+):role");
                                if (!roleNameMatch.Success)
                                {
                                    service.RegisterServiceEvent(Journaling.EventType.Error, "Ошибка получения роли IAM для логирования событий отправки SMS", "Возможно, изменился формат названия роли. Ожидался формат \"arn:aws:iam::xxxxxxxxxxxx:role\".");
                                    return string.Empty;
                                }
                                else
                                {
                                    var name = $"sns/{getClientCloudWatchLogs().Config.RegionEndpoint.SystemName }/{roleNameMatch.Groups[1].Value}/DirectPublishToPhoneNumber";
                                    return name;
                                }
                            }
                            else
                            {
                                service.RegisterServiceEvent(Journaling.EventType.Warning, "Ошибка получения данных о настройках", "В настройках Amazon SNS / Text messaging (SMS) не задана роль IAM для ведения логов в Amazon CloudWatch. Зайдите в консоль управления в раздел \"Amazon SNS / Text messaging (SMS) / Manage text messaging preferences\", проверьте пункт \"IAM role for CloudWatch Logs access\". Если в нем есть только ссылка \"Create IAM role\", следует перейти по ней и настроить роль IAM.");
                                return string.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            service.RegisterServiceEvent(Journaling.EventType.Warning, "Неожиданная ошибка получения данных о настройках", null, ex);
                            Debug.WriteLine(ex);
                            return string.Empty;
                        }
                    }, TimeSpan.FromMinutes(5));

                    if (!string.IsNullOrEmpty(logGroupName))
                    {
                        try
                        {
                            var taskRr = getClientCloudWatchLogs().FilterLogEventsAsync(new FilterLogEventsRequest() { LogGroupName = logGroupName, FilterPattern = "{ $.notification.messageId = \"" + message.ExternalID + "\" }" });
                            taskRr.Wait();
                            var rr = taskRr.Result;
                            if (rr != null && rr.Events != null && rr.Events.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(rr.Events.First().Message) && rr.Events.First().Message.Contains("\"status\":\"SUCCESS\""))
                                {
                                    message.IsHandled = true;
                                    message.IsSuccess = true;
                                }
                            }
                        }
                        catch (AmazonCloudWatchLogsException ex)
                        {
                            if (ex.ErrorCode == "AccessDeniedException")
                            {
                                service.RegisterServiceEvent(Journaling.EventType.Warning, "Ошибка получения статуса доставки", "Доступ запрещен - возможно, у используемого IAM-ключа недостаточно прав для чтения данных из Amazon CloudWatch", ex);
                            }
                            else throw;
                            //if (ex.)
                            //Amazon.CloudWatchLogs.AmazonCloudWatchLogsException: User: arn: aws: iam::036187153096:user / Dombonus is not authorized to perform: logs: FilterLogEvents on resource: arn: aws: logs: us - west - 2:036187153096:log - group:sns / us - west - 2 / 036187153096 / DirectPublishToPhoneNumber:log - 
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (HandledException ex)
            {
                service.RegisterServiceEvent(Journaling.EventType.Error, "Amazon SNS - ошибка отправки сообщения", ex.Message, ex.InnerException);
                message.IsError = false;
                message.ErrorText = ex.Message;
            }
            catch (Exception ex)
            {
                service.RegisterServiceEvent(Journaling.EventType.Error, "Telegram - необработанная ошибка отправки сообщения", null, ex);
                Debug.WriteLine(ex.ToString());
                message.IsError = false;
                message.ErrorText = "Необработанная ошибка во время отправки сообщения";
            }
        }

        private AmazonSimpleNotificationServiceClient getClientSNS()
        {
            if (_clientCloudWatchLogs == null) throw new System.InvalidOperationException("Коннектор не был корректно инициализирован.");
            return _clientSNS;
        }

        private AmazonCloudWatchLogsClient getClientCloudWatchLogs()
        {
            if (_clientCloudWatchLogs == null) throw new System.InvalidOperationException("Коннектор не был корректно инициализирован.");
            return _clientCloudWatchLogs;
        }

        public string ConnectorName
        {
            get => "Amazon SNS";
        }
        #endregion

        void IDisposable.Dispose() => OnStop();

        uint IPoolObjectOrdered.OrderInPool { get; } = 10;
    }
}
