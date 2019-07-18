using OnUtils.Application.Configuration;
using OnUtils.Application.DB;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnWeb.Plugins.Adminmain.Model
{
    using WebCoreModule;

    public class AdminMainModelInfoPage
    {
        public AdminMainModelInfoPage() : this(new CoreConfiguration<WebApplicationBase>(), new WebCoreConfiguration())
        {
        }

        public AdminMainModelInfoPage(CoreConfiguration<WebApplicationBase> appCoreConfiguration, WebCoreConfiguration webCoreConfiguration)
        {
            AppCoreConfiguration = appCoreConfiguration;
            WebCoreConfiguration = webCoreConfiguration;
        }

        public List<Role> Roles { get; set; }

        public List<SelectListItem> ModulesList { get; set; } = new List<SelectListItem>();

        public CoreConfiguration<WebApplicationBase> AppCoreConfiguration { get; }

        public WebCoreConfiguration WebCoreConfiguration { get; }
    }
}
