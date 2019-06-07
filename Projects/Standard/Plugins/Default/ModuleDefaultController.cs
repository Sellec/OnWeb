using System.Web.Mvc;

namespace OnWeb.Plugins.Default
{
    using CoreBind.Modules;

    public class ModuleDefaultController : ModuleControllerUser<ModuleDefault>
    {
        public override ActionResult Index()
        {
            this.RegisterEventWithCode(System.Net.HttpStatusCode.InternalServerError, "1313");
            return this.display("Index.cshtml");
        }

    }
}
