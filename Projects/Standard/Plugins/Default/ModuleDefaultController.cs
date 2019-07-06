using System;
using System.Web.Mvc;

namespace OnWeb.Plugins.Default
{
    using CoreBind.Modules;

    public class ModuleDefaultController : ModuleControllerUser<ModuleDefault>
    {
        public override ActionResult Index()
        {
            AppCore.Get<MessagingEmail.IEmailService>().SendMailToDeveloper("123123", "1231313");
            return this.display("Index.cshtml");
        }
    }
}
