using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Messaging.Email
{
    class Service : ServiceBase<Message>, IService
    {
        public Service() : base("Email", "Email".GenerateGuid())
        {
            IsSupportsIncoming = false;
            IsSupportsOutcoming = true;
            IsSupportsCurrentStatusInfo = false;
        }

        #region Отправка
        bool IService.sendMail(string name_from, string email_from, string name_to, string email_to, Encoding data_charset, Encoding send_charset, string subject, string body, List<int> files)
        {
            //todo setError(null);

            //email_to = "dev@alikm.com";
            email_from = "dombonus@yandex.ru";

            var message = new Message()
            {
                From = new Contact<string>(name_from, email_from),
                To = new List<Contact<string>>() { new Contact<string>(name_to, email_to) },
                Subject = subject,
                Body = body,
            };

            return RegisterMessage(message);
        }

        bool IService.SendToAdmin(string subject, string body)
        {

            return ((IService)this).sendMail(
                "Почтовый робот сайта",
                GetNoReplyAddress(),
                "admin",
                AppCore.Config.Get("helpform_email", ""),
                null, null,
                subject,
                body
            );
        }

        bool IService.SendMailFromSite(string nameTo, string emailTo, string subject, string body, List<int> files)
        {
            return ((IService)this).sendMail("Почтовый робот сайта", GetNoReplyAddress(), nameTo, emailTo, null, null, subject, body, files);
        }

        bool IService.sendMailToDeveloper(string subject, string body, List<int> files)
        {
            return ((IService)this).sendMail("Почтовый робот сайта", GetNoReplyAddress(), "Developers", "dev@alikm.com", null, null, subject, body, files);
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
            var address = "no-reply@site.site";
            if (AppCore.ServerUrl != null) address = "no-reply@" + AppCore.ServerUrl.Authority;

            return address;
        }

        #endregion
    }
}
