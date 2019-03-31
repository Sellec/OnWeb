using System.Collections.Generic;

namespace OnWeb.Plugins.ModuleAdminmain.Model
{
    using Core.Modules;

    public class Routing
    {
        public List<ModuleCore> Modules;
        public Dictionary<int, int> RoutesMain;
        public Dictionary<int, int> RoutesAdditional;
        public Dictionary<int, int> RoutesOld;
    }
}
