using System;

namespace OnWeb.Plugins.MessagingEmail.Connectors
{
    /// <summary>
    /// Хранит настройки подключения к smtp-серверу.
    /// </summary>
    public class SmtpServerSettings
    {
        /// <summary>
        /// Адрес сервера.
        /// </summary>
        public Uri Server { get; set; }

        /// <summary>
        /// Логин для подключения к smtp-серверу.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль для подключения к smtp-серверу.
        /// </summary>
        public string Password { get; set; }
    }
}
