using OnUtils.Application.Modules;
using System.Web.Mvc;

namespace OnWeb.Modules.Admin
{
    using Core.Modules;

    public sealed class ModuleAdminController : ModuleControllerUser<ModuleAdmin>
    {
        [ModuleAction(null, ModulesConstants.PermissionManageString)]
        public override ActionResult Index()
        {
            return display("admin.cshtml");
        }
    }

}
