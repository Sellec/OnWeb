using System.ComponentModel.DataAnnotations;

namespace OnWeb.Modules.MessagingEmail.Model
{
    /// <summary>
    /// Хранит настройки подключения к smtp-серверу.
    /// </summary>
    public class SmtpServerSettings
    {
        /// <summary>
        /// См. <see cref="Components.SmtpServerSettings.Server"/>
        /// </summary>
        [Display(Name = "Адрес smtp-сервера")]
        [Required]
        [MaxLength(200)]
        public string Server { get; set; }

        /// <summary>
        /// См. <see cref="Components.SmtpServerSettings.IsSecure"/>
        /// </summary>
        [Display(Name = "Использовать подключение по SSL?")]
        [Required]
        public bool IsSecure { get; set; }

        /// <summary>
        /// См. <see cref="Components.SmtpServerSettings.Port"/>
        /// </summary>
        [Display(Name = "Порт подключения (необязателен для заполнения. По-умолчанию для небезопасного подключения используется порт 80, для безопасного 587)")]
        public int? Port { get; set; }

        /// <summary>
        /// См. <see cref="Components.SmtpServerSettings.Login"/>
        /// </summary>
        [Display(Name = "Логин для подключения к smtp-серверу")]
        [Required]
        [MaxLength(200)]
        [DataType(DataType.Text)]
        public string Login { get; set; }

        /// <summary>
        /// См. <see cref="Components.SmtpServerSettings.Password"/>
        /// </summary>
        [Display(Name = "Пароль для подключения к smtp-серверу")]
        [Required]
        [MaxLength(200)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}