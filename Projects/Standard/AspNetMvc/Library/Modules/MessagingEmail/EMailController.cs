using Newtonsoft.Json;
using OnUtils.Application.Configuration;
using OnUtils.Application.Messaging;
using OnUtils.Application.Messaging.MessageHandlers;
using OnUtils.Application.Modules.CoreModule;
using System.Linq;

namespace OnWeb.Modules.MessagingEmail
{
    using Core.Configuration;

    using Model;

    class EMailController : Core.Modules.ModuleControllerAdmin<EMailModule, Configuration, Configuration>
    {
        protected override void ConfigurationViewFill(Configuration viewModelForFill, out string viewName)
        {
            viewName = "ModuleSettings.cshtml";

            var handlers = AppCore.AppConfig.MessageHandlersSettings.
                Where(x => x.TypeFullName.StartsWith(typeof(MessageHandlers.SmtpServer).Namespace)).
                Select(x => new { x.TypeFullName, Settings = JsonConvert.DeserializeObject<MessageHandlers.SmtpServerSettings>(x.SettingsSerialized) }).
                ToList();

            var smtp = handlers.Where(x => x.TypeFullName.EndsWith("." + nameof(MessageHandlers.SmtpServer))).Select(x => x.Settings).FirstOrDefault();

            viewModelForFill.ApplyConfiguration(smtp);
        }

        protected override ModuleConfiguration<EMailModule> ConfigurationSaveCustom(Configuration formData, out string outputMessage)
        {
            var handlers = AppCore.AppConfig.MessageHandlersSettings.ToDictionary(x => x.TypeFullName, x => x);

            handlers.Remove(typeof(MessageHandlers.SmtpServer).FullName);
            if (formData.IsUseSmtp)
            {
                handlers[typeof(MessageHandlers.SmtpServer).FullName] = new MessageHandlerSettings()
                {
                    TypeFullName = typeof(MessageHandlers.SmtpServer).FullName,
                    SettingsSerialized = JsonConvert.SerializeObject(new MessageHandlers.SmtpServerSettings()
                    {
                        Server = formData?.Smtp?.Server,
                        IsSecure = formData?.Smtp?.IsSecure ?? false,
                        Port = formData?.Smtp?.Port,
                        Login = formData?.Smtp?.Login,
                        Password = formData?.Smtp?.Password
                    })
                };
            }

            var cfg = AppCore.Get<CoreModule<WebApplication>>().GetConfigurationManipulator().GetEditable<CoreConfiguration<WebApplication>>();

            cfg.MessageHandlersSettings = handlers.Values.ToList();

            AppCore.Get<CoreModule<WebApplication>>().GetConfigurationManipulator().ApplyConfiguration(cfg);
            AppCore.Get<MessagingManager<WebApplication>>().UpdateHandlersFromSettings();

            return base.ConfigurationSaveCustom(formData, out outputMessage);
        }
    }
}
