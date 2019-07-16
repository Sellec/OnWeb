using System.Collections.Generic;

namespace OnWeb.Plugins.Adminmain.Model
{
    using Core.Modules;

    public class Module
    {
        public Module(IModuleCore module)
        {
            Id = module.IdModule;
            Caption = module.Caption;
            Type = module.QueryType.FullName;
            UniqueName = module.UniqueName.ToString();
            UrlName = module.UrlName;
            IsConfigAllowed = module.ControllerAdmin() != null;
        }

        public int Id { get; set; }
        public string Caption { get; set; }
        public string Type { get; set; }
        public string UniqueName { get; set; }
        public string UrlName { get; set; }

        public bool IsConfigAllowed { get; set; }
    }

    public class Modules
    {
        public List<Module> Registered;
    }
}