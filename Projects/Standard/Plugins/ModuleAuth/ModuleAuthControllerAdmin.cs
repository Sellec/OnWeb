using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Auth
{
    public class ModuleAuthControllerAdmin : AdminForModules.ModuleAdminController<ModuleAuth, Configuration.CoreContext, Model.ConfigurationSaveModel>
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
            if (model.RoleGuest.HasValue) ApplicationCore.Instance.Config["RoleGuest"] = model.RoleGuest;
            if (model.RoleUser.HasValue) ApplicationCore.Instance.Config["RoleUser"] = model.RoleUser; ;
            if (model.EventLoginSuccess.HasValue) ApplicationCore.Instance.Config["eventLoginSuccess"] = model.EventLoginSuccess;
            if (model.EventLoginError.HasValue) ApplicationCore.Instance.Config["eventLoginError"] = model.EventLoginError;
            if (model.EventLoginUpdate.HasValue) ApplicationCore.Instance.Config["eventLoginUpdate"] = model.EventLoginUpdate;
            if (model.EventLogout.HasValue) ApplicationCore.Instance.Config["eventLogout"] = model.EventLogout;

            ApplicationCore.Instance.ConfigurationSave();
        }
    }
}