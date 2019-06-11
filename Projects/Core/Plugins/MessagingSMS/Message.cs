namespace OnWeb.Plugins.MessagingSMS
{
    using Core.Messaging;

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
