using System.Collections.Generic;

using System.Web.Mvc;

namespace OnWeb.Plugins.ModuleAdminmain.Model
{
    using Core.Modules;

    public class RoutingModule
    {
        public ModuleCore Module;

        public IEnumerable<RouteInfo> Routes;

        public IEnumerable<SelectListItem> ModulesActions;

        public IEnumerable<SelectListItem> RoutingTypes;

    }
}