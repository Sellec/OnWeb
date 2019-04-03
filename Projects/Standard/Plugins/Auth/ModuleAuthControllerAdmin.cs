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
            if (model.RoleGuest.HasValue) AppCore.Config["RoleGuest"] = model.RoleGuest;
            if (model.RoleUser.HasValue) AppCore.Config["RoleUser"] = model.RoleUser; ;
            if (model.EventLoginSuccess.HasValue) AppCore.Config["eventLoginSuccess"] = model.EventLoginSuccess;
            if (model.EventLoginError.HasValue) AppCore.Config["eventLoginError"] = model.EventLoginError;
            if (model.EventLoginUpdate.HasValue) AppCore.Config["eventLoginUpdate"] = model.EventLoginUpdate;
            if (model.EventLogout.HasValue) AppCore.Config["eventLogout"] = model.EventLogout;

            AppCore.ConfigurationSave();
        }
    }
}