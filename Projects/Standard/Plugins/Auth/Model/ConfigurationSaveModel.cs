namespace OnWeb.Plugins.Auth.Model
{
    using CoreBind.Modules.Configuration;

    public class ConfigurationSaveModel : SaveModel
    {
        public int? EventLoginSuccess { get; set; }

        public int? EventLoginError { get; set; }

        public int? EventLoginUpdate { get; set; }

        public int? EventLogout { get; set; }

        public int? RoleUser { get; set; }

        public int? RoleGuest { get; set; }
    }
}