using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Auth
{
    using Core.Configuration;
    using Core.DB;
    using Core.Items;
    using Core.Modules;
    using Core.Users;
    using Core.Exceptions;
    using CoreBind.Modules;
    using Core.Types;
    using Core.Modules;
    using Core.Routing;
    using CoreBind.Modules;
    using CoreBind.Routing;
    using Core.DB;
    using Core.Journaling;
    using OnWeb.Plugins.Auth.Model;

    public class ModuleAuthControllerAdmin : ModuleControllerAdmin<ModuleAuth, CoreContext, Model.ConfigurationSaveModel>
    {
        public override ActionResult Configuration()
        {
            var model = new Design.Model.ModuleSettings();
            model.Roles = (from p in DB.Role orderby p.NameRole ascending select p).ToList();
            model.EventTypes = (from p in DB.UserLogHistoryEventType orderby p.NameEventType ascending select p).ToList();

            return View("ModuleSettings.cshtml", model);
        }

        protected override ModuleConfiguration<ModuleAuth> ConfigurationSaveCustom(ConfigurationSaveModel formData, out string outputMessage)
        {
            var cfg = Module.GetConfigurationManipulator().GetEditable<ModuleConfiguration>();

            if (formData.RoleGuest.HasValue) cfg.RoleGuest = formData.RoleGuest.Value;
            if (formData.RoleUser.HasValue) cfg.RoleUser = formData.RoleUser.Value;
            if (formData.EventLoginSuccess.HasValue) cfg.EventLoginSuccess = formData.EventLoginSuccess.Value;
            if (formData.EventLoginError.HasValue) cfg.EventLoginError = formData.EventLoginError.Value;
            if (formData.EventLoginUpdate.HasValue) cfg.EventLoginUpdate = formData.EventLoginUpdate.Value;
            if (formData.EventLogout.HasValue) cfg.EventLogout = formData.EventLogout.Value;

            outputMessage = null;
            return cfg;
        }
    }
}