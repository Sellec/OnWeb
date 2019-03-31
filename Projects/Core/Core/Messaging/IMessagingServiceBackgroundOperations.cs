using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Описывает сервис отправки/приема сообщений с поддержкой фоновых процессов отправки/приема.
    /// </summary>
    public interface IMessagingServiceBackgroundOperations : IMessagingService
    {
        /// <summary>
        /// Обрабатывает входящие сообщения, если режим приема входящих сообщений поддерживается сервисом. См. <see cref="IMessagingService.IsSupportsIncoming"/>.
        /// </summary>
        void ExecuteIncoming();

        /// <summary>
        /// Обрабатывает исходящие сообщения, если режим отправки исходящих сообщений поддерживается сервисом. См. <see cref="IMessagingService.IsSupportsOutcoming"/>.
        /// </summary>
        void ExecuteOutcoming();

    }
}
