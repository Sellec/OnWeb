using OnUtils.Application.Configuration;
using OnUtils.Application.DB;
using System;
using System.Linq;

namespace OnWeb.Plugins.Auth
{
    using CoreBind.Modules;
    using Model;

    public class ModuleAuthControllerAdmin : ModuleControllerAdmin<ModuleAuth, Design.Model.ModuleSettings, Configuration>
    {
        protected override void ConfigurationViewFill(Design.Model.ModuleSettings viewModelForFill, out string viewName)
        {
            using (var db = Module.CreateUnitOfWork())
            {
                viewModelForFill.ApplyConfiguration(Module.GetConfiguration<ModuleConfiguration>());
                viewModelForFill.EventTypes = (from p in db.UserLogHistoryEventType orderby p.NameEventType ascending select p).ToList();
                viewModelForFill.EventTypes.Insert(0, new Core.DB.UserLogHistoryEventType() { IdEventType = 0, NameEventType = "Не выбрано" });
                viewName = "ModuleSettings.cshtml";
            }
        }

        protected override ModuleConfiguration<ModuleAuth> ConfigurationSaveCustom(Configuration formData, out string outputMessage)
        {
            var cfg = Module.GetConfigurationManipulator().GetEditable<ModuleConfiguration>();

            if (formData.EventLoginSuccess.HasValue) cfg.EventLoginSuccess = formData.EventLoginSuccess.Value;
            if (formData.EventLoginError.HasValue) cfg.EventLoginError = formData.EventLoginError.Value;
            if (formData.EventLoginUpdate.HasValue) cfg.EventLoginUpdate = formData.EventLoginUpdate.Value;
            if (formData.EventLogout.HasValue) cfg.EventLogout = formData.EventLogout.Value;

            outputMessage = null;
            return cfg;
        }
    }
}