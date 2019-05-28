using System;
using System.Web.Mvc;

namespace OnWeb.Plugins.Developing
{
    using CoreBind.Modules;

    public class ModuleDevelopingController : ModuleControllerUser<ModuleDeveloping>
    {
        public override ActionResult Index()
        {
            throw new NotImplementedException();
        }

        public ActionResult TestError500()
        {
            Session["asdasd"] = DateTime.Now;
            var iss = HttpContext.Session.IsReadOnly;
            Debug.WriteLineNoLog("{0}", iss);

            var d = DateTime.Now;
            while ((DateTime.Now - d).TotalSeconds <= 5) { }

            //throw new Exception("тестовая ошибка");

            return Content("123123123");
        }
    }
}