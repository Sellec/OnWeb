using System.Web.Mvc;

namespace OnWeb.Plugins.Default
{
    using CoreBind.Modules;

    public class ModuleDefaultController : ModuleControllerUser<ModuleDefault>
    {
        public override ActionResult Index()
        {
            return this.display("Index.cshtml");
        }

    }
}
