namespace OnWeb.Plugins.Auth.Model
{
    public class Configuration : CoreBind.Modules.Configuration.SaveModel
    {
        public void ApplyConfiguration(ModuleConfiguration source)
        {
            EventLoginSuccess = source?.EventLoginSuccess;
            EventLoginError = source?.EventLoginError;
            EventLoginUpdate = source?.EventLoginUpdate;
            EventLogout = source?.EventLogout;
            RoleUser = source?.RoleUser;
            RoleGuest = source?.RoleGuest;
        }

        public int? EventLoginSuccess { get; set; }

        public int? EventLoginError { get; set; }

        public int? EventLoginUpdate { get; set; }

        public int? EventLogout { get; set; }

        public int? RoleUser { get; set; }

        public int? RoleGuest { get; set; }
    }
}