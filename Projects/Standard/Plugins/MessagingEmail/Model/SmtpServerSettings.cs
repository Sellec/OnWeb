using System.ComponentModel.DataAnnotations;

namespace OnWeb.Plugins.MessagingEmail.Model
{
    /// <summary>
    /// Хранит настройки подключения к smtp-серверу.
    /// </summary>
    public class SmtpServerSettings
    {
        /// <summary>
        /// См. <see cref="Connectors.SmtpServerSettings.Server"/>
        /// </summary>
        [Display(Name = "Адрес smtp-сервера")]
        [Required]
        [MaxLength(200)]
        [DataType(DataType.Url)]
        public string Server { get; set; }

        /// <summary>
        /// См. <see cref="Connectors.SmtpServerSettings.Login"/>
        /// </summary>
        [Display(Name = "Логин для подключения к smtp-серверу")]
        [Required]
        [MaxLength(200)]
        [DataType(DataType.Text)]
        public string Login { get; set; }

        /// <summary>
        /// См. <see cref="Connectors.SmtpServerSettings.Password"/>
        /// </summary>
        [Display(Name = "Пароль для подключения к smtp-серверу")]
        [Required]
        [MaxLength(200)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}