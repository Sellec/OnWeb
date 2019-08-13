using System.Collections.Generic;
using System.Web.Mvc;

namespace OnWeb.Modules.Adminmain.Model
{
    using Core.Modules;

    public class RoutingModule
    {
        public IModuleCore Module;

        public IEnumerable<RouteInfo> Routes;

        public IEnumerable<SelectListItem> ModulesActions;

        public IEnumerable<SelectListItem> RoutingTypes;

    }
}