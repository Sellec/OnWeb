using System.Web.Mvc;

namespace OnWeb.Plugins.Default
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
