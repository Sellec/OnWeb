using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Plugins.Auth
{
    public class ModuleConfiguration : Core.Configuration.ModuleConfiguration<ModuleAuth>
    {
        public int EventLoginSuccess
        {
            get => Get("eventLoginSuccess", 0);
            set => Set("eventLoginSuccess", value);
        }

        public int EventLoginError
        {
            get => Get("eventLoginError", 0);
            set => Set("eventLoginError", value);
        }

        public int EventLoginUpdate
        {
            get => Get("eventLoginUpdate", 0);
            set => Set("eventLoginUpdate", value);
        }

        public int EventLogout
        {
            get => Get("eventLogout", 0);
            set => Set("eventLogout", value);
        }

        public int RoleUser
        {
            get => Get("roleUser", 0);
            set => Set("roleUser", value);
        }

        public int RoleGuest
        {
            get => Get("roleGuest", 0);
            set => Set("roleGuest", value);
        }
    }
}
