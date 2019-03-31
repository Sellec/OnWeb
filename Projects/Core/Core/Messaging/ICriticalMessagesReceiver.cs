using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Представляет сервис, обрабатывающий критические сообщения.
    /// </summary>
    public interface ICriticalMessagesReceiver
    {
        /// <summary>
        /// Отправляет сообщение администратору системы.
        /// </summary>
        /// <param name="subject">Заголовок сообщения.</param>
        /// <param name="body">Тело сообщения.</param>
        void SendToAdmin(string subject, string body);
    }
}
