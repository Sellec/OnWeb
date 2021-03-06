﻿using System.Web.Routing;

namespace OnWeb.CoreBind.Routing
{
    public class RouteWithDefaults : Route
    {
        class RoutesDefaults
        {
            public RoutesDefaults(ApplicationCore core)
            {
                var moduleID = core.Config.IdModuleDefault;
                var module = core.GetModulesManager().GetModule(moduleID) ?? core.GetModulesManager().GetModule<Plugins.Default.ModuleDefault>();

                Controller = module.IdModule.ToString();
            }

            public string Controller
            {
                get;
            }

            public string Action
            {
                get => nameof(Modules.ModuleControllerUser<Plugins.CoreModule.CoreModule>.Index);
            }

            public System.Web.Mvc.UrlParameter Url
            {
                get => System.Web.Mvc.UrlParameter.Optional;
            }
        }

        class RoutesDefaultsWithLanguage : RoutesDefaults
        {
            public RoutesDefaultsWithLanguage(ApplicationCore core) : base(core)
            {
                Language = core.Get<Core.Languages.Manager>().GetUserLanguage().ShortName;
            }

            public string Language
            {
                get;
            }
        }

        private readonly ApplicationCore _core = null;
        private readonly bool _defaultsWithLanguage;

        public RouteWithDefaults(ApplicationCore core, string url, bool defaultsWithLanguage, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, new RouteValueDictionary(defaultsWithLanguage ? new RoutesDefaultsWithLanguage(core) : new RoutesDefaults(core)), constraints, dataTokens, routeHandler)
        {
            _core = core;
            _defaultsWithLanguage = defaultsWithLanguage;
        }

        public void UpdateDefaults()
        {
            Defaults = new RouteValueDictionary(_defaultsWithLanguage ? new RoutesDefaultsWithLanguage(_core) : new RoutesDefaults(_core));
        }
    }
}
