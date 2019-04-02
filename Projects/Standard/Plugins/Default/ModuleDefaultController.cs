using System.Web.Mvc;

namespace OnWeb.Plugins.ModuleDefault
{
    using CoreBind.Modules;

    public class ModuleDefaultController : ModuleControllerUser<ModuleDefault>
    {
        public ActionResult Index(string part = null)
        {
            return this.display("Index.cshtml");
        }

    }
}
