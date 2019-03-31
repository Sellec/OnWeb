using System.Collections.Generic;

namespace OnWeb.Plugins.ModuleAdminmain.Model
{
    public class Module
    {
        public Module(Core.Modules.ModuleCore module)
        {
            Id = module.ID;
            Caption = module.Caption;
            Type = module.GetType().FullName;
            UrlName = module.UrlName;
        }

        public int Id { get; set; }
        public string Caption { get; set; }
        public string Type { get; set; }
        public string UrlName { get; set; }
    }

    public class Modules
    {
        public List<Module> Registered;
    }
}