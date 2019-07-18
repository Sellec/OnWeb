using OnUtils.Application.Messaging;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Plugins.MessagingEmail
{
    using Core.Messaging;

    /// <summary>
    /// Представляет сервис отправки электронных писем (Email).
    /// </summary>
    public interface IEmailService : IMessagingService, ICriticalMessagesReceiver
    {
        /// <summary>
        /// Отправка письма получателю <paramref name="nameTo"/> с адресом <paramref name="emailTo"/> с темой <paramref name="subject"/>, с текстом <paramref name="body"/>.
        /// </summary>
        void SendMailFromSite(string nameTo, string emailTo, string subject, string body, ContentType contentType, List<int> files = null);

        void SendMailToDeveloper(string subject, string body, ContentType contentType, List<int> files = null);

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
        void SendMail(string name_from, string email_from, string name_to, string email_to, Encoding data_charset, Encoding send_charset, string subject, string body, ContentType contentType, List<int> files = null);

    }
}
