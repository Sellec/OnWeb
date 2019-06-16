using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Plugins.MessagingEmail
{
    using Core.Messaging;

    class EmailService : ServiceBase<EmailMessage>, IEmailService
    {
        public EmailService() : base("Email", "Email".GenerateGuid())
        {
            IsSupportsIncoming = false;
            IsSupportsOutcoming = true;
            IsSupportsCurrentStatusInfo = false;
        }

        #region Отправка
        void IEmailService.SendMail(string name_from, string email_from, string name_to, string email_to, Encoding data_charset, Encoding send_charset, string subject, string body, List<int> files)
        {
            var message = new EmailMessage()
            {
                From = new Contact<string>(name_from, email_from),
                To = new List<Contact<string>>() { new Contact<string>(name_to, email_to) },
                Subject = subject,
                Body = body,
            };

            RegisterMessage(message);
        }

        void IEmailService.SendMailFromSite(string nameTo, string emailTo, string subject, string body, List<int> files)
        {
            ((IEmailService)this).SendMail("Почтовый робот сайта", GetNoReplyAddress(), nameTo, emailTo, null, null, subject, body, files);
        }

        void IEmailService.SendMailToDeveloper(string subject, string body, List<int> files)
        {
            ((IEmailService)this).SendMail("Почтовый робот сайта", GetNoReplyAddress(), "Developers", "test@test.com", null, null, subject, body, files);
        }

        /**
        * Рассылка писем по рассылке номер $IdSubscription с темой $subject, с текстом $body.
        * 
        * @param int        $IdSubscription
        * @param string     $subject
        * @param string     $body
        */
        public bool sendMailSubscription(int IdSubscription, string subject, string body)
        {
            try
            {
                //todo setError(null);
                var result = AppCore.Get<ISubscriptionsManager>().send(IdSubscription, subject, body);
                //todo if (!result) setError(SubscriptionsManager.getError());
                return result;
            }
            catch (Exception ex)
            {
                //todo setError(ex.Message);
                return false;
            }
        }

        private string GetNoReplyAddress()
        {
            var address = AppCore.Config.ReturnEmail;
            if (!string.IsNullOrEmpty(address)) return address;

            address = "no-reply@localhost";
            if (AppCore.ServerUrl != null) address = "no-reply@" + AppCore.ServerUrl.Host;

            return address;
        }

        void ICriticalMessagesReceiver.SendToAdmin(string subject, string body)
        {
            ((IEmailService)this).SendMail(
                "Почтовый робот сайта",
                GetNoReplyAddress(),
                "admin",
                AppCore.Config.CriticalMessagesEmail,
                null, null,
                subject,
                body
            );
        }

        #endregion
    }
}
