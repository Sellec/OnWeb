using System.ComponentModel.DataAnnotations;

namespace OnWeb.Modules.reCAPTCHA.Model
{
    public class Configuration : Core.Modules.Configuration.SaveModel
    {
        /// <summary>
        /// См. <see cref="ModuleReCaptchaConfiguration.IsEnabledValidation"/>
        /// </summary>
        [Display(Name = "Использовать механизм reCAPTCHA?")]
        public bool IsEnabledValidation { get; set; }

        /// <summary>
        /// См. <see cref="ModuleReCaptchaConfiguration.PublicKey"/>
        /// </summary>
        [Display(Name = "Публичный ключ для проверки пользователя сервисом")]
        public string PublicKey { get; set; }

        /// <summary>
        /// См. <see cref="ModuleReCaptchaConfiguration.PrivateKey"/>
        /// </summary>
        [Display(Name = "Приватный ключ для проверки кода, отправленного из формы после проверки пользователя сервисом")]
        public string PrivateKey { get; set; }
    }
}