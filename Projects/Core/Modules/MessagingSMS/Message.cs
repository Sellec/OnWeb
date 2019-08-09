using OnUtils.Application.Messaging;

namespace OnWeb.Modules.MessagingSMS
{
    /// <summary>
    /// Описывает СМС-сообщение.
    /// </summary>
    public class Message : MessageBase
    {
        /// <summary>
        /// Номер получателя сообщения.
        /// </summary>
        public Contact<string> To { get; set; }
    }
}
