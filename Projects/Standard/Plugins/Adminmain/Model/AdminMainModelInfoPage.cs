using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Adminmain.Model
{
    public class AdminMainModelInfoPage
    {
        public AdminMainModelInfoPage() : this(new Core.Configuration.WebCoreConfiguration())
        {
        }

        public AdminMainModelInfoPage(Core.Configuration.WebCoreConfiguration configuration)
        {
            Configuration = configuration;
        }

        public List<SelectListItem> ModulesList { get; set; } = new List<SelectListItem>();

        public Core.Configuration.WebCoreConfiguration Configuration { get; }
    }
}