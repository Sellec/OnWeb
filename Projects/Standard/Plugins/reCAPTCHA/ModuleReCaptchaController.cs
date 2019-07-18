namespace OnWeb.Plugins.reCAPTCHA
{
    using Core.Configuration;
    using Model;

    class ModuleReCaptchaController : CoreBind.Modules.ModuleControllerAdmin<ModuleReCaptcha, Configuration, Configuration>
    {
        protected override void ConfigurationViewFill(Configuration viewModelForFill, out string viewName)
        {
            viewName = "ModuleSettings.cshtml";

            var cfg = Module.GetConfiguration<ModuleReCaptchaConfiguration>();

            viewModelForFill.IsEnabledValidation = cfg.IsEnabledValidation;
            viewModelForFill.PublicKey = cfg.PublicKey;
            viewModelForFill.PrivateKey = cfg.PrivateKey;
        }

        protected override ModuleConfiguration<ModuleReCaptcha> ConfigurationSaveCustom(Configuration formData, out string outputMessage)
        {
            outputMessage = null;

            if (formData.IsEnabledValidation && string.IsNullOrEmpty(formData.PublicKey))
                ModelState.AddModelError(nameof(formData.PublicKey), "Публичный ключ должен быть указан, если проверка включена.");

            if (formData.IsEnabledValidation && string.IsNullOrEmpty(formData.PrivateKey))
                ModelState.AddModelError(nameof(formData.PrivateKey), "Приватный ключ должен быть указан, если проверка включена.");

            var cfg = Module.GetConfigurationManipulator().GetEditable<ModuleReCaptchaConfiguration>();

            if (ModelState.IsValid)
            {
                cfg.IsEnabledValidation = formData.IsEnabledValidation;
                cfg.PublicKey = formData.PublicKey;
                cfg.PrivateKey = formData.PrivateKey;
            }

            return cfg;
        }
    }
}
