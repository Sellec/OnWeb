using System.Collections.Generic;

namespace OnWeb.Modules.Adminmain.Model
{
    using Core.Modules;

    public class Routing
    {
        public List<IModuleCore> Modules;
        public Dictionary<int, int> RoutesMain;
        public Dictionary<int, int> RoutesAdditional;
        public Dictionary<int, int> RoutesOld;
    }
}
