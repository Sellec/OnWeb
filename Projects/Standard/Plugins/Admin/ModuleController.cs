using OnUtils.Application.Modules;
using System.Web.Mvc;

namespace OnWeb.Plugins.Admin
{
    using CoreBind.Modules;

    class ModuleController : ModuleControllerUser<ModuleAdmin>
    {
        [ModuleAction(null, ModulesConstants.PermissionManageString)]
        public ActionResult Index()
        {
            return display("admin.cshtml");
        }
    }

}
