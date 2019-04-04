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

    public class ModuleAuthControllerAdmin : ModuleControllerAdmin<ModuleAuth, CoreContext, Model.ConfigurationSaveModel>
    {
        public override ActionResult Configuration()
        {
            var model = new Design.Model.ModuleSettings();
            model.Roles = (from p in DB.Role orderby p.NameRole ascending select p).ToList();
            model.EventTypes = (from p in DB.UserLogHistoryEventType orderby p.NameEventType ascending select p).ToList();

            return View("ModuleSettings.cshtml", model);
        }

        protected override void ConfigurationSaveCustom(Model.ConfigurationSaveModel model)
        {
            var cfg = Module.GetConfigurationManipulator().GetEditable<ModuleConfiguration>();

            if (model.RoleGuest.HasValue) cfg.RoleGuest = model.RoleGuest.Value;
            if (model.RoleUser.HasValue) cfg.RoleUser = model.RoleUser.Value;
            if (model.EventLoginSuccess.HasValue) cfg.EventLoginSuccess = model.EventLoginSuccess.Value;
            if (model.EventLoginError.HasValue) cfg.EventLoginError = model.EventLoginError.Value;
            if (model.EventLoginUpdate.HasValue) cfg.EventLoginUpdate = model.EventLoginUpdate.Value;
            if (model.EventLogout.HasValue) cfg.EventLogout = model.EventLogout.Value;

            Module.GetConfigurationManipulator().ApplyConfiguration(cfg);
        }
    }
}