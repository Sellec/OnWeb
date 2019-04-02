using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Adminmain.Model
{
    public class AdminMainModelInfoPage
    {
        public AdminMainModelInfoPage() : this(new Core.Configuration.CoreConfiguration())
        {
        }

        public AdminMainModelInfoPage(Core.Configuration.CoreConfiguration configuration)
        {
            Configuration = configuration;
        }

        public List<SelectListItem> ModulesList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> PagesList { get; set; } = new List<SelectListItem>();

        public Core.Configuration.CoreConfiguration Configuration { get; }
    }
}