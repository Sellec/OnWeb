namespace OnWeb.Modules.Auth.Model
{
    public class Configuration : Core.Modules.Configuration.SaveModel
    {
        public void ApplyConfiguration(ModuleConfiguration source)
        {
            EventLoginSuccess = source?.EventLoginSuccess;
            EventLoginError = source?.EventLoginError;
            EventLoginUpdate = source?.EventLoginUpdate;
            EventLogout = source?.EventLogout;
        }

        public int? EventLoginSuccess { get; set; }

        public int? EventLoginError { get; set; }

        public int? EventLoginUpdate { get; set; }

        public int? EventLogout { get; set; }
    }
}