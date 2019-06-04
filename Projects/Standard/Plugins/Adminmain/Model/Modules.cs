using System.Collections.Generic;

namespace OnWeb.Plugins.Adminmain.Model
{
    public class Module
    {
        public Module(Core.Modules.ModuleCore module)
        {
            Id = module.ID;
            Caption = module.Caption;
            Type = module.QueryType.FullName;
            UrlName = module.UrlName;
            IsConfigAllowed = module.ControllerAdmin() != null;
        }

        public int Id { get; set; }
        public string Caption { get; set; }
        public string Type { get; set; }
        public string UrlName { get; set; }

        public bool IsConfigAllowed { get; set; }
    }

    public class Modules
    {
        public List<Module> Registered;
    }
}