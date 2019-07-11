using OnUtils.Application.Messaging;
using System;

namespace OnWeb.Plugins.MessagingSMS
{
    /// <summary>
    /// Представляет сервис отправки СМС.
    /// </summary>
    public interface IService : IMessagingService
    {
        /// <summary>
        /// Отправка смс-сообщения на номер телефона <paramref name="phoneTo"/> с текстом <paramref name="messageText"/>.
        /// </summary>
        /// <param name="phoneTo">Должен являться корректным номером телефона. В противном случае сгенерируется исключение <see cref="ArgumentException"/>.</param>
        /// <param name="messageText">Текст сообщения.</param>
        /// <returns>Возвращает результат постановки сообщения в очередь.</returns>
        void SendMessage(string phoneTo, string messageText);

        /// <summary>
        /// Отправка смс-сообщения на номер телефона администратора сайта текстом <paramref name="messageText"/>.
        /// </summary>
        /// <param name="messageText">Текст сообщения.</param>
        /// <returns>Возвращает результат постановки сообщения в очередь.</returns>
        void SendToAdmin(string messageText);
    }
}
