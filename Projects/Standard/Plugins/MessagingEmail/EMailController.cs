using Newtonsoft.Json;
using System;
using System.Linq;

namespace OnWeb.Plugins.MessagingEmail
{
    using Model;
    using OnWeb.Core.Configuration;

    class EMailController : CoreBind.Modules.ModuleControllerAdmin<EMailModule, Configuration, Configuration>
    {
        protected override void ConfigurationViewFill(Configuration viewModelForFill, out string viewName)
        {
            viewName = "ModuleSettings.cshtml";

            var connectors = AppCore.Config.ConnectorsSettings.
                Where(x => x.ConnectorTypeName.StartsWith(typeof(Connectors.SmtpServer).Namespace)).
                Select(x => new { x.ConnectorTypeName, Settings = JsonConvert.DeserializeObject<Connectors.SmtpServerSettings>(x.SettingsSerialized) }).
                ToList();

            var smtp = connectors.Where(x => x.ConnectorTypeName.EndsWith("." + nameof(Connectors.SmtpServer))).Select(x => x.Settings).FirstOrDefault();

            viewModelForFill.ApplyConfiguration(smtp);
        }

        protected override ModuleConfiguration<EMailModule> ConfigurationSaveCustom(Configuration formData, out string outputMessage)
        {
            var connectors = AppCore.Config.ConnectorsSettings.ToDictionary(x => x.ConnectorTypeName, x => x);

            connectors.Remove(typeof(Connectors.SmtpServer).FullName);
            if (formData.IsUseSmtp)
            {
                connectors[typeof(Connectors.SmtpServer).FullName] = new Core.Messaging.Connectors.ConnectorSettings()
                {
                    ConnectorTypeName = typeof(Connectors.SmtpServer).FullName,
                    SettingsSerialized = JsonConvert.SerializeObject(new Connectors.SmtpServerSettings()
                    {
                        Server = Uri.TryCreate(formData?.Smtp?.Server, UriKind.Absolute, out var uri2) ? uri2 : null,
                        Login = formData?.Smtp?.Login,
                        Password = formData?.Smtp?.Password
                    })
                };
            }

            var cfg = AppCore.GetModulesManager().GetModule<WebCoreModule.WebCoreModule>().GetConfigurationManipulator().GetEditable<WebCoreConfiguration>();

            cfg.ConnectorsSettings = connectors.Values.ToList();

            AppCore.GetModulesManager().GetModule<WebCoreModule.WebCoreModule>().GetConfigurationManipulator().ApplyConfiguration(cfg);
            AppCore.Get<Core.Messaging.MessagingManager>().UpdateConnectorsFromSettings();

            return base.ConfigurationSaveCustom(formData, out outputMessage);
        }
    }
}
