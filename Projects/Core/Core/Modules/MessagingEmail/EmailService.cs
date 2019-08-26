using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Modules.MessagingEmail
{
    using Messaging;

    /// <summary>
    /// Представляет сервис отправки электронных писем (Email).
    /// </summary>
    public class EmailService : MessageServiceBase<EmailMessage>, ICriticalMessagesReceiver
    {
        /// <summary>
        /// </summary>
        public EmailService() : base("Email", "Email".GenerateGuid())
        {
            IsSupportsIncoming = false;
            IsSupportsOutcoming = true;
            IsSupportsCurrentStatusInfo = false;
        }

        #region Отправка
        /// <summary>
        /// Отправка письма на указанный адрес, с указанной темой, с указанным текстом.
        /// </summary>
        /// <param name="name_from">Имя отправителя</param>
        /// <param name="email_from">email отправителя</param>
        /// <param name="name_to">имя получателя</param>
        /// <param name="email_to">email получателя</param>
        /// <param name="data_charset">кодировка переданных данных</param>
        /// <param name="send_charset">кодировка письма</param>
        /// <param name="subject">тема письма</param>
        /// <param name="body">текст письма</param>
        /// <param name="files">Прикрепленные файлы</param>
        /// <returns></returns>
        public void SendMail(string name_from, string email_from, string name_to, string email_to, Encoding data_charset, Encoding send_charset, string subject, string body, ContentType contentType, List<int> files = null)
        {
            if (contentType == ContentType.Text) body = body.Replace("\n", "\n<br />");

            var message = new EmailMessage()
            {
                From = new Contact<string>(name_from, email_from),
                To = new List<Contact<string>>() { new Contact<string>(name_to, email_to) },
                Subject = subject,
                Body = body,
            };

            RegisterOutcomingMessage(message);
        }

        /// <summary>
        /// Отправка письма получателю <paramref name="nameTo"/> с адресом <paramref name="emailTo"/> с темой <paramref name="subject"/>, с текстом <paramref name="body"/>.
        /// </summary>
        public void SendMailFromSite(string nameTo, string emailTo, string subject, string body, ContentType contentType, List<int> files = null)
        {
            SendMail("Почтовый робот сайта", GetNoReplyAddress(), nameTo, emailTo, null, null, subject, body, contentType, files);
        }

        public void SendMailToDeveloper(string subject, string body, ContentType contentType, List<int> files = null)
        {
            SendMail("Почтовый робот сайта", GetNoReplyAddress(), AppCore.WebConfig.DeveloperEmail, AppCore.WebConfig.DeveloperEmail, null, null, subject, body, contentType, files);
        }

        /**
        * Рассылка писем по рассылке номер $IdSubscription с темой $subject, с текстом $body.
        * 
        * @param int        $IdSubscription
        * @param string     $subject
        * @param string     $body
        */
        public void SendMailSubscription(int idSubscription, string subject, string body, ContentType contentType)
        {
            AppCore.Get<ISubscriptionsManager>().send(idSubscription, subject, body, contentType);
        }

        private string GetNoReplyAddress()
        {
            var address = AppCore.WebConfig.ReturnEmail;
            if (!string.IsNullOrEmpty(address)) return address;

            address = "no-reply@localhost";
            if (AppCore.ServerUrl != null) address = "no-reply@" + AppCore.ServerUrl.Host;

            return address;
        }

        void ICriticalMessagesReceiver.SendToAdmin(string subject, string body)
        {
            SendMail(
                "Почтовый робот сайта",
                GetNoReplyAddress(),
                "admin",
                AppCore.WebConfig.CriticalMessagesEmail,
                null, null,
                subject,
                body,
                ContentType.Text
            );
        }

        #endregion
    }
}
