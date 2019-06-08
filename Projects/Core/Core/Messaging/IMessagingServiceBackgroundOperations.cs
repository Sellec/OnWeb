using System;

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
        [ApiIrreversible]
        void ExecuteIncoming();

        /// <summary>
        /// Обрабатывает исходящие сообщения, если режим отправки исходящих сообщений поддерживается сервисом. См. <see cref="IMessagingService.IsSupportsOutcoming"/>.
        /// </summary>
        [ApiIrreversible]
        void ExecuteOutcoming();

    }
}
