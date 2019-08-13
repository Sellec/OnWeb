using OnUtils.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace OnWeb.CoreBind.Razor
{
    using Core.Modules;
    using Modules.WebCoreModule;

    public abstract class TraceViewPage<TModel> : WebViewPage<TModel>
    {
        private Guid PageUniqueIdentifier = Guid.NewGuid();

        public TraceViewPage()
        {

        }

        protected override string NormalizeLayoutPagePath(string layoutPagePath)
        {
            IEnumerable<string> searchLocations = null;
            var engine = ViewEngines.Engines.Where(x => x is Providers.ResourceProvider).Select(x => x as Providers.ResourceProvider).FirstOrDefault();
            if (engine != null)
            {
                var d = this.Context;
                var path = engine.GetFilePath(engine.GetModuleNameFromContext(this.ViewContext.Controller.ControllerContext), layoutPagePath, true, out searchLocations);
                if (!string.IsNullOrEmpty(path)) return path;
            }

            Debug.WriteLine("LayoutPage not found '{0}' for view '{1}'.", layoutPagePath, this.VirtualPath);
            throw new System.IO.FileNotFoundException($"Для представления '{this.VirtualPath}' не найден макет '{layoutPagePath}'.\r\nРасположения для поиска:\r\n - " + string.Join(";\r\n - ", searchLocations) + ".", layoutPagePath);
        }

    }

}
