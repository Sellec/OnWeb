using System;

namespace OnWeb.Modules.MessagingSMS
{
    using Messaging;

    /// <summary>
    /// Представляет сервис отправки СМС.
    /// </summary>
    public abstract class Service : MessageServiceBase<Message>
    {
        /// <summary>
        /// </summary>
        protected Service(string serviceName, Guid serviceID) : base(serviceName, serviceID)
        {
        }
        
        /// <summary>
        /// Отправка смс-сообщения на номер телефона <paramref name="phoneTo"/> с текстом <paramref name="messageText"/>.
        /// </summary>
        /// <param name="phoneTo">Должен являться корректным номером телефона. В противном случае сгенерируется исключение <see cref="ArgumentException"/>.</param>
        /// <param name="messageText">Текст сообщения.</param>
        /// <returns>Возвращает результат постановки сообщения в очередь.</returns>
        public abstract void SendMessage(string phoneTo, string messageText);

        /// <summary>
        /// Отправка смс-сообщения на номер телефона администратора сайта текстом <paramref name="messageText"/>.
        /// </summary>
        /// <param name="messageText">Текст сообщения.</param>
        /// <returns>Возвращает результат постановки сообщения в очередь.</returns>
        public abstract void SendToAdmin(string messageText);
    }
}
