using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnWeb.Core.Configuration;
using OnWeb.Modules.WebCoreModule;
using OnWeb.Core.Modules;
using System.Web.WebPages;
using System.Web.Mvc;

namespace System
{
    public static class WebViewPageExtension
    {rwerwer
        /// <summary>
        /// Предоставляет доступ к настройкам сайта.
        /// </summary>
        public static WebCoreConfiguration GetConfig(this WebViewPage page)
        {
            return ((IModuleCore)page.ViewData["Module"]).GetAppCore().WebConfig;
        }
    }
}
