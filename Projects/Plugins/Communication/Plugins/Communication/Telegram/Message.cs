namespace OnWeb.Plugins.Communication.Telegram
{
    using Core.Messaging;

    /// <summary>
    /// Описывает сообщение в Telegram.
    /// </summary>
    public class Message : MessageBase
    {
        /// <summary>
        /// Идентификатор получателя в Telegram.
        /// </summary>
        public Contact<string> To { get; set; }
    }
}
