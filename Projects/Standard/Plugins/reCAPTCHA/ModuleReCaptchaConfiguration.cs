namespace OnWeb.Plugins.reCAPTCHA
{
    using Core.Configuration;

    /// <summary>
    /// Настройки модуля <see cref="ModuleReCaptcha"/>.
    /// </summary>
    public class ModuleReCaptchaConfiguration  : ModuleConfiguration<ModuleReCaptcha>
    {
        /// <summary>
        /// Указывает, включена ли проверка reCAPTCHA во время валидации моделей.
        /// </summary>
        public bool IsEnabledValidation
        {
            get => Get("IsEnabledValidation", false);
            set => Set("IsEnabledValidation", value);
        }

        /// <summary>
        /// Приватный ключ для проверки кода, отправленного из формы после проверки пользователя сервисом.
        /// </summary>
        public string PrivateKey
        {
            get => Get("PrivateKey", "");
            set => Set("PrivateKey", value);
        }
    }
}