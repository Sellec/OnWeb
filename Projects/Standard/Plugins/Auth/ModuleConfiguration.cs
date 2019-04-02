using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Auth
{
    public class ModuleConfiguration : Core.Configuration.ModuleConfiguration<ModuleAuth>
    {
        public int EventLoginSuccess { get; set; }

        public int EventLoginError { get; set; }

        public int EventLoginUpdate { get; set; }

        public int EventLogout { get; set; }

        public int RoleUser { get; set; }

        public int RoleGuest { get; set; }
    }
}