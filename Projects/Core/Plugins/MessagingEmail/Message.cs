using System.Collections.Generic;

namespace OnWeb.Plugins.MessagingEmail
{
    using Core.Messaging;

    /// <summary>
    /// Описывает сообщение электронной почты.
    /// </summary>
    public class Message : MessageBase
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
