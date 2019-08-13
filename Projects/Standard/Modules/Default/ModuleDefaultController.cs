using System.Web.Mvc;

namespace OnWeb.Modules.Default
{
    using Core.Modules;

    public class ModuleDefaultController : ModuleControllerUser<ModuleDefault>
    {
        public override ActionResult Index()
        {
            return View("Index.cshtml");
        }
    }
}
