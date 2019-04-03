namespace OnWeb.Plugins.Communication.Telegram
{
    using Core.Messaging;

    /// <summary>
    /// Представляет сервис отправки сообщений в Telegram.
    /// </summary>
    public interface IService : IMessagingService, ICriticalMessagesReceiver
    {
        /// <summary>
        /// Отправка сообщения от имени бота администратору сайта.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        /// <returns>Результат регистрации сообщения в системе обмена сообщениями.</returns>
        bool SendToAdmin(string message);
    }
}
