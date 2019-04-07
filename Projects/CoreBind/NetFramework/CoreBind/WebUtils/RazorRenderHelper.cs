using System;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnWeb.Core.WebUtils
{
    using CoreBind.Modules;

    /// <summary>
    /// </summary>
    public static class RazorRenderHelper
    {
        class FakeController<TModule> : ModuleControllerUser<TModule> where TModule : Modules.ModuleCore<TModule>
        {
            public override ActionResult Index()
            {
                throw new NotImplementedException();
            }
        }

        public class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }

        public static string RenderView(Modules.ModuleCore module, string template, object model = null)
        {
            try
            {
                if (!template.EndsWith(".tpl") && !template.EndsWith(".cshtml") && !template.EndsWith(".vbhtml")) template += ".cshtml";
                if (template.EndsWith(".tpl")) template = template.Replace(".tpl", ".cshtml");
                if (!template.StartsWith("~/")) template = "~/" + template;

                //это есть только в контроллерах.
                //module.displayPrepare(model);

                //ViewData.Model = model;
                using (var sw = new System.IO.StringWriter())
                {
                    var filename = "/";
                    var uri = "http://localhost/";
                    var context = HttpContext.Current ?? new HttpContext(new HttpRequest(filename, uri, ""), new HttpResponse(new StringWriter()));

                    var contextWrapper = new HttpContextWrapper(context);
                    var routeData = new RouteData();
                    routeData.Values["controller"] = "fake";

                    var controller = Activator.CreateInstance(typeof(FakeController<>).MakeGenericType(module.GetType())) as ModuleControllerBase;
                    var method = controller.GetType().GetMethod(nameof(ModuleControllerUser<Plugins.CoreModule.CoreModule>.InitController), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    method.Invoke(controller, new object[] { module });

                    var controllerContext = new ControllerContext(new RequestContext(contextWrapper, routeData), controller as Controller);

                    var viewResult = ViewEngines.Engines.FindView(controllerContext, template, null);
                    if (viewResult.View == null) throw new ArgumentException($"Представление '{template}' не найдено.", nameof(template));
                    var viewData = new ViewDataDictionary(model);
                    viewData["Module"] = module;
                    var viewContext = new ViewContext(controllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

    }
}

