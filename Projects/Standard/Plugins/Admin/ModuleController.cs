using OnUtils.Application.Modules;
using System.Web.Mvc;

namespace OnWeb.Plugins.ModuleAdmin
{
    using CoreBind.Modules;

    class ModuleController : ModuleControllerUser<Module>
    {
        [ModuleAction(null, Constants.PermissionManageString)]
        public ActionResult Index()
        {
            return display("admin.cshtml");
        }
    }

}
