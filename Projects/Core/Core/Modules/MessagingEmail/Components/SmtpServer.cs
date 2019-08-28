using Newtonsoft.Json;
using OnUtils.Application.Journaling;
using OnUtils.Application.Messaging.Messages;
using System;
using System.Net.Mail;
using System.Text;

namespace OnWeb.Modules.MessagingEmail.Components
{
    using Messaging.Components;
    using OnWeb.Messaging;

    /// <summary>
    /// Предоставляет возможность отправки электронной почты через smtp-сервер. Поддерживается только <see cref="SmtpDeliveryMethod.Network"/>.
    /// </summary>
    public sealed class SmtpServer : OutcomingMessageSender<EmailMessage>
    {
        private SmtpClient _client = null;

        /// <summary>
        /// </summary>
        public SmtpServer() : base("SMTP-сервер", 20)
        {
        }

        #region OutcomingMessageSender<EmailMessage>
        /// <summary>
        /// </summary>
        protected override bool OnStartComponent()
        {
            if (_client != null) throw new InvalidOperationException("Компонент уже инициализирован.");

            try
            {
                var settingsParsed = !string.IsNullOrEmpty(SerializedSettings) ? JsonConvert.DeserializeObject<SmtpServerSettings>(SerializedSettings) : new SmtpServerSettings();

                if (string.IsNullOrEmpty(settingsParsed.Server)) return false;

                var client = new SmtpClient()
                {
                    Host = settingsParsed.Server,
                    Port = settingsParsed.Port.HasValue ? settingsParsed.Port.Value : (settingsParsed.IsSecure ? 587 : 80),
                    EnableSsl = settingsParsed.IsSecure,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(settingsParsed.Login, settingsParsed.Password),
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

        /// <summary>
        /// См. <see cref="OutcomingMessageSender{TMessage}.OnSend(MessageInfo{TMessage}, MessageServiceBase{TMessage})"/>.
        /// </summary>
        protected override bool OnSend(MessageInfo<EmailMessage> message, MessageServiceBase<EmailMessage> service)
        {
            try
            {
                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(message.Message.From.ContactData, string.IsNullOrEmpty(message.Message.From.Name) ? message.Message.From.ContactData : message.Message.From.Name),
                    SubjectEncoding = Encoding.UTF8,
                    Subject = message.Message.Subject,
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    Body = message.Message.Body?.ToString(),
                };

                var developerEmail = AppCore.WebConfig.DeveloperEmail;
                if (Debug.IsDeveloper && string.IsNullOrEmpty(developerEmail)) return false;

                message.Message.To.ForEach(x => mailMessage.To.Add(new MailAddress(Debug.IsDeveloper ? developerEmail : x.ContactData, string.IsNullOrEmpty(x.Name) ? x.ContactData : x.Name)));

                try
                {
                    getClient().Send(mailMessage);
                    message.StateType = MessageStateType.Completed;
                    return true;
                }
                catch (SmtpException ex)
                {
                    var canBeResend = true;
                    service.RegisterServiceEvent(EventType.Error, "SMTP - ошибка отправки письма", null, ex);
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
                                message.StateType = MessageStateType.Completed;
                                return true;

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
            catch (FormatException ex)
            {
                service.RegisterServiceEvent(EventType.Error, "SMTP - ошибка отправки письма", "Некорректный Email-адрес", ex);
                message.StateType = MessageStateType.Error;
                message.State = "Некорректный Email-адрес";
                return true;
            }
            catch (Exception ex)
            {
                service.RegisterServiceEvent(EventType.Error, "SMTP - ошибка отправки письма", null, ex);
                return false;
            }
        }

        private SmtpClient getClient()
        {
            if (_client == null) throw new InvalidOperationException("Компонент не был корректно инициализирован.");
            return _client;
        }
        #endregion
    }
}
