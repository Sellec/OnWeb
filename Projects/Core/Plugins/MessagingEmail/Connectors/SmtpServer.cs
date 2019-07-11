using Newtonsoft.Json;
using OnUtils.Application.Journaling;
using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.Connectors;
using OnUtils.Architecture.ObjectPool;
using System;
using System.Net.Mail;
using System.Text;

namespace OnWeb.Plugins.MessagingEmail.Connectors
{
    using Core;

    /// <summary>
    /// Предоставляет возможность отправки электронной почты через smtp-сервер. Поддерживается только <see cref="SmtpDeliveryMethod.Network"/>.
    /// </summary>
    public sealed class SmtpServer : CoreComponentBase, IConnectorBase<EmailMessage>, IDisposable
    {
        private SmtpClient _client = null;

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
            var client = _client;
            _client = null;

            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }
        #endregion

        #region IConnectorBase<Message>
        /// <summary>
        /// См. <see cref="IConnectorBase{TMessage}.Init(string)"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Возникает, если коннектор уже был инициализирован.</exception>
        bool IConnectorBase<EmailMessage>.Init(string connectorSettings)
        {
            if (_client != null) throw new InvalidOperationException("Коннектор уже инициализирован.");

            try
            {
                var settings = !string.IsNullOrEmpty(connectorSettings) ? JsonConvert.DeserializeObject<SmtpServerSettings>(connectorSettings) : new SmtpServerSettings();

                if (settings.Server == null) return false;
                if (!settings.Server.IsAbsoluteUri) return false;
                if (settings.Server.Scheme != "http" && settings.Server.Scheme != "https") return false;

                var client = new SmtpClient()
                {
                    Host = settings.Server.Host,
                    Port = !settings.Server.IsDefaultPort ? settings.Server.Port : (settings.Server.Scheme == "http" ? 80 : (settings.Server.Scheme == "https" ? 587 : 0)),
                    EnableSsl = settings.Server.Scheme == "https",
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(settings.Login, settings.Password),
                };

                _client = client;

                return true;
            }
            catch (Exception)
            {
                // todo здесь должна быть возможность писать в журнал. Пока не реализовано - делаем throw дальше, попадет в общий журнал.
                throw;
            }
        }

        void IConnectorBase<EmailMessage>.Send(ConnectorMessage<EmailMessage> message, IMessagingService service)
        {
            try
            {
                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(message.MessageBody.From.ContactData, string.IsNullOrEmpty(message.MessageBody.From.Name) ? message.MessageBody.From.ContactData : message.MessageBody.From.Name),
                    SubjectEncoding = Encoding.UTF8,
                    Subject = message.MessageBody.Subject,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    Body = message.MessageBody.Body?.ToString(),
                };

                var developerEmail = AppCore.GetWebConfig().DeveloperEmail;
                if (Debug.IsDeveloper && string.IsNullOrEmpty(developerEmail)) return;

                message.MessageBody.To.ForEach(x => mailMessage.To.Add(new MailAddress(Debug.IsDeveloper ? developerEmail : x.ContactData, string.IsNullOrEmpty(x.Name) ? x.ContactData : x.Name)));

                try
                {
                    getClient().Send(mailMessage);
                    message.HandledState = ConnectorMessageStateType.Sent;
                }
                catch (SmtpException ex)
                {
                    var canBeResend = true;
                    service.RegisterServiceEvent(EventType.Error, "Gmail - ошибка отправки письма", null, ex);
                    //if (ex.Message.Contains("Message rejected: Email address is not verified"))
                    //{
                    //    var match = System.Text.RegularExpressions.Regex.Match(ex.Message, "Message rejected: Email address is not verified. The following identities failed the check in region ([^:]+): (.+)");
                    //    if (match.Success)
                    //    {
                    //        throw new Exception($"Добавьте адрес '{match.Groups[2].Value}' в раздел 'Identity Management/Email Addresses', либо снимите ограничения на отправку писем в регионе '{match.Groups[1].Value}'.");
                    //    }
                    //    canBeResend = false;
                    //}

                    if (canBeResend)
                    {
                        switch (ex.StatusCode)
                        {
                            case SmtpStatusCode.ServiceClosingTransmissionChannel:
                            case SmtpStatusCode.ServiceNotAvailable:
                            case SmtpStatusCode.TransactionFailed:
                            case SmtpStatusCode.GeneralFailure:
                                try
                                {
                                    _client.Dispose();
                                    _client = null;
                                }
                                catch { }

                                getClient().Send(mailMessage);
                                message.HandledState = ConnectorMessageStateType.Sent;
                                break;

                            default:
                                throw;
                        }
                    }
                    else throw;
                }

                //todo добавить прикрепление файлов в отправку писем.
                //if (is_array($files) && count($files) > 0)
                //	foreach ($files as $k=>$v)
                //	{
                //		if (isset($v['url"]) && isset($v['name"]))
                //			$mail->AddAttachment(SITE_PATH.$v['url"], $v['name"].'.'.pathinfo($v['url"], PATHINFO_EXTENSION));
                //		else if (isset($v['path"]) && isset($v['name"]))	
                //			$mail->AddAttachment($v['path"], $v['name"]);
                //	}

                //$success = $mail->send();
            }
            catch(FormatException)
            {
                message.HandledState = ConnectorMessageStateType.Error;
                message.State = "Некорректный Email-адрес";
            }
            catch (Exception)
            {
                message.HandledState = ConnectorMessageStateType.Error;
                message.State = "Необработанная ошибка во время отправки сообщения";
            }
        }

        private SmtpClient getClient()
        {
            if (_client == null) throw new InvalidOperationException("Коннектор не был корректно инициализирован.");
            return _client;
        }

        string IConnectorBase<EmailMessage>.ConnectorName
        {
            get => "SMTP-сервер";
        }
        #endregion

        void IDisposable.Dispose() => OnStop();

        uint IPoolObjectOrdered.OrderInPool { get; } = 20;
    }
}
