using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Базовый класс сообщения. Все специфические типы сообщений сервисов должны наследоваться от него.
    /// </summary>
    public abstract class MessageBase
    {
        /// <summary>
        /// Тема сообщения.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Тело сообщения.
        /// </summary>
        public object Body { get; set; }

        /// <summary>
        /// Тип сообщения.
        /// </summary>
        public byte IdMessageType { get; set; }
    }
}
