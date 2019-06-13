using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Core.Configuration
{
    /// <summary>
    /// Класс конфигурации. При создании экземпляра объекта через метод Create ядра <see cref="ApplicationCore"/> автоматически заполняется значениями настроек ядра.
    /// </summary>
#pragma warning disable CS1591 // todo внести комментарии.
    public class CoreConfiguration : ModuleConfiguration<Plugins.CoreModule.CoreModule>
    {
        [Display(Name = "Название сайта"), Required(ErrorMessage = "Название сайта не может быть пустым"), MaxLength(200)]
        public string SiteFullName
        {
            get => Get("site_title", "");
            set => Set("site_title", value);
        }

        [Display(Name = "Модуль, загружаемый по-умолчанию"), ModuleValidation(ErrorMessage = "Модуль, открывающийся по-умолчанию, должен быть выбран.")]
        public int IdModuleDefault
        {
            get => Get("index_module", 0);
            set => Set("index_module", value);
        }

        [Display(Name = "Основной контактный email сайта"), Required(ErrorMessage = "Основной контактный email не может быть пустым")]
        [EmailAddress(ErrorMessage = "Неправильно указан основной контактный email сайта")]
        public string helpform_email
        {
            get => Get("helpform_email", "");
            set => Set("helpform_email", value);
        }

        [Display(Name = "Режим регистрации на сайте")]
        public Types.RegisterMode register_mode
        {
            get => Get("register_mode", Types.RegisterMode.Immediately);
            set => Set("register_mode", value);
        }

        [Display(Name = "Режим авторизации на сайте")]
        public Users.eUserAuthorizeAllowed userAuthorizeAllowed
        {
            get => Get("userAuthorizeAllowed", Users.eUserAuthorizeAllowed.OnlyEmail);
            set => Set("userAuthorizeAllowed", value);
        }

        [Display(Name = "Информация перед регистрацией")]
        public string site_reginfo
        {
            get => Get("site_reginfo", "");
            set => Set("site_reginfo", value);
        }

        [Display(Name = "Информация после входа в систему")]
        public string site_loginfo
        {
            get => Get("site_loginfo", "");
            set => Set("site_loginfo", value);
        }

        [Display(Name = "Контактная информация (на странице обратной связи)")]
        public string help_info
        {
            get => Get("help_info", "");
            set => Set("help_info", value);
        }

        [Display(Name = "SEO Описание сайта(description)")]
        public string site_descr
        {
            get => Get("site_descr", "");
            set => Set("site_descr", value);
        }

        [Display(Name = "SEO Ключевые слова (keywords)")]
        public string site_keys
        {
            get => Get("site_keys", "");
            set => Set("site_keys", value);
        }

        /// <summary>
        /// Основной системный язык.
        /// </summary>
        [Display(Name = "Основной системный язык")]
        public int IdSystemLanguage
        {
            get => Get("IdSystemLanguage", -1);
            set => Set("IdSystemLanguage", value);
        }

        /// <summary>
        /// Email разработчика сайта, который должен получать уведомления, связанные с отслеживанием работы кода.
        /// </summary>
        [Display(Name = "Email разработчика сайта")]
        public string DeveloperEmail
        {
            get => Get("DeveloperEmail", "");
            set => Set("DeveloperEmail", value);
        }

        /// <summary>
        /// Настройки коннекторов приёма/отправки сообщений.
        /// </summary>
        /// <seealso cref="Messaging.Connectors.IConnectorBase{TMessage}"/>
        /// <seealso cref="Messaging.MessagingManager"/>
        public List<Messaging.Connectors.ConnectorSettings> ConnectorsSettings
        {
            get => JsonConvert.DeserializeObject<List<Messaging.Connectors.ConnectorSettings>>(Get("ConnectorsSettings", "")) ?? new List<Messaging.Connectors.ConnectorSettings>();
            set => Set("ConnectorsSettings", value == null ? "" : JsonConvert.SerializeObject(value));
        }
    }
}
