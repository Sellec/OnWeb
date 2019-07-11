using OnUtils.Application.Messaging;
using System.Collections.Generic;

namespace OnWeb.Plugins.MessagingEmail
{
    /// <summary>
    /// Описывает сообщение электронной почты.
    /// </summary>
    public class EmailMessage : MessageBase
    {
        /// <summary>
        /// Отправитель сообщения.
        /// </summary>
        public Contact<string> From { get; set; }

        /// <summary>
        /// Список получателей (кому).
        /// </summary>
        public List<Contact<string>> To { get; set; } = new List<Contact<string>>();

        /// <summary>
        /// Список получателей (копия).
        /// </summary>
        public List<Contact<string>> CopyTo { get; set; } = new List<Contact<string>>();
    }
}
