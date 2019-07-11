using OnUtils.Application.Languages;
using System.Web.Routing;

namespace OnWeb.CoreBind.Routing
{
    public class RouteWithDefaults : Route
    {
        class RoutesDefaults
        {
            public RoutesDefaults(WebApplicationBase core)
            {
                var moduleID = core.WebConfig.IdModuleDefault;
                var module = core.GetModulesManager().GetModule(moduleID) ?? core.GetModulesManager().GetModule<Plugins.Default.ModuleDefault>();

                Controller = module.IdModule.ToString();
            }

            public string Controller
            {
                get;
            }

            public string Action
            {
                get => nameof(Modules.ModuleControllerUser<Plugins.WebCoreModule.WebCoreModule>.Index);
            }

            public System.Web.Mvc.UrlParameter Url
            {
                get => System.Web.Mvc.UrlParameter.Optional;
            }
        }

        class RoutesDefaultsWithLanguage : RoutesDefaults
        {
            public RoutesDefaultsWithLanguage(WebApplicationBase core) : base(core)
            {
                Language = core.Get<Manager>().GetUserLanguage().ShortName;
            }

            public string Language
            {
                get;
            }
        }

        private readonly WebApplicationBase _core = null;
        private readonly bool _defaultsWithLanguage;

        public RouteWithDefaults(WebApplicationBase core, string url, bool defaultsWithLanguage, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
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
