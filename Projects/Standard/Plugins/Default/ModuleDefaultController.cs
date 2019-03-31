using System.Web.Mvc;

namespace OnWeb.Plugins.Default
{
    public class ModuleDefaultController : ModuleController<ModuleDefault>
    {
        [ModuleAction(null, ModuleCore.ACCESSUSER)]
        public ActionResult Index(string part = null)
        {
            var data = UserManager.Instance.getData();
            return this.display("customerIndex.cshtml", data);
        }

    }
}
