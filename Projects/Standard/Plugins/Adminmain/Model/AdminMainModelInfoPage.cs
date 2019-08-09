using OnUtils.Application.Configuration;
using OnUtils.Application.DB;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnWeb.Modules.Adminmain.Model
{
    using WebCoreModule;

    public class AdminMainModelInfoPage
    {
        public AdminMainModelInfoPage() : this(new CoreConfiguration<WebApplication>(), new WebCoreConfiguration())
        {
        }

        public AdminMainModelInfoPage(CoreConfiguration<WebApplication> appCoreConfiguration, WebCoreConfiguration webCoreConfiguration)
        {
            AppCoreConfiguration = appCoreConfiguration;
            WebCoreConfiguration = webCoreConfiguration;
        }

        public List<Role> Roles { get; set; }

        public List<SelectListItem> ModulesList { get; set; } = new List<SelectListItem>();

        public CoreConfiguration<WebApplication> AppCoreConfiguration { get; }

        public WebCoreConfiguration WebCoreConfiguration { get; }
    }
}
