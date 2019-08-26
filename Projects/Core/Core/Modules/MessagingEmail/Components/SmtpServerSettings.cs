namespace OnWeb.Modules.MessagingEmail.Components
{
    /// <summary>
    /// Хранит настройки подключения к smtp-серверу.
    /// </summary>
    public class SmtpServerSettings
    {
        /// <summary>
        /// Адрес сервера.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Указывает, следует ли использовать SSL.
        /// </summary>
        public bool IsSecure { get; set; }

        /// <summary>
        /// Порт подключения. Если не задан, то при <see cref="IsSecure"/> равном false используется порт 80, при <see cref="IsSecure"/> равном true используется порт 587.
        /// </summary>
        public int? Port { get; set; }

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
