using OnUtils.Application.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnWeb.CoreBind.Routing
{
    using Core.DB;

    class RouteHandler : MvcRouteHandler, IRouteConstraint
    {
        private ConcurrentDictionary<string, Routing> _dbCache = new ConcurrentDictionary<string, Routing>();
        private readonly WebApplicationBase _core = null;

        public RouteHandler(WebApplicationBase core)
        {
            _core = core;
        }

        private Routing GetMatchedRoute(RouteValueDictionary routeValues)
        {
            var url = UriExtensions.MakeRelativeFromUrl(routeValues.Values.Select(x => (x?.ToString() ?? "").TrimEnd('/')).First().ToString()).ToLower();

            if (url.Trim() == "/") return null;

            if (_dbCache.TryGetValue(url, out Routing data)) return data;

            using (var db = new OnUtils.Data.UnitOfWork<Routing>())
            {
                // Вложенный запрос работает долю секунды. А предыдущий вариант с условием и сортировкой внутри одного селекта работает значительно дольше.
                var queryRoutes = db.DataContext.ExecuteQuery<Routing>($@"
                    SELECT *
                    FROM [dbo].[UrlTranslation]
                    WHERE IdTranslation = (
                        SELECT top (1) [IdTranslation]
                        FROM [dbo].[UrlTranslation]
                        WHERE ([UrlFull] LIKE @Url + '%' AND [IsFixedLength] <> 1) OR ([UrlFull] = @Url AND [IsFixedLength] = 1)
                        ORDER BY CASE WHEN (1 = [IdTranslationType]) THEN 0 ELSE 1 END ASC, [IsFixedLength] ASC, LEN([UrlFull]) DESC, [DateChange] DESC, [IdTranslation] DESC
                    )
                ", new { Url = url });

                var route = queryRoutes.FirstOrDefault();

                //EF генерит неправильное определение для url.StartsWith(x.UrlFull) в виде CHARINDEX вместо LIKE, из-за чего запрос выполняется дольше секунды. 
                //Пока что не получилось заставить его делать как надо.
                //var routes = DB.Routing
                //    .Where(x => (url.StartsWith(x.UrlFull) && !x.IsFixedLength) || (url == x.UrlFull && x.IsFixedLength))
                //    .OrderBy(x => x.IdRoutingType == RoutingType.eTypes.Main ? 0 : 1)
                //    .ThenBy(x => x.IsFixedLength)
                //    .ThenByDescending(x => x.UrlFull.Length)
                //    .ThenByDescending(x => x.DateChange)
                //    .Take(2);

                //var list = routes.ToList();

                if (route != null)
                {
                    _dbCache.SetWithExpiration(url, route, TimeSpan.FromMilliseconds(5));
                    return route;
                }
            }

            return null;
        }

        private string GetExistingFile(RouteValueDictionary routeValues)
        {
            var fileRelative = routeValues["url"] as string;
            if (string.IsNullOrEmpty(fileRelative)) return null;

            var fileRelativeQithoutQuery = fileRelative.Split(new char[] { '?' }, 2)[0];
            var fileReal = ((Providers.ResourceProvider)_core.Get<Core.Storage.ResourceProvider>()).GetFilePath(null, fileRelativeQithoutQuery, false, out IEnumerable<string> searchLocations);
            if (!string.IsNullOrEmpty(fileReal))
            {
                var directoryRoot = System.IO.Path.GetDirectoryName(_core.ApplicationWorkingFolder);
                var directoryFile = System.IO.Path.GetDirectoryName(fileReal);
                if (directoryFile == directoryRoot) return fileReal;
            }
            return null;
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (routeDirection == RouteDirection.IncomingRequest)
            {
                if (GetMatchedRoute(values) != null) return true;
                if (!string.IsNullOrEmpty(GetExistingFile(values))) return true;
            }

            return false;
        }

        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var route = GetMatchedRoute(requestContext.RouteData.Values);
            if (route != null)
            {
                if (route.IdRoutingType == RoutingType.eTypes.Old)
                {
                    using (var db = new OnUtils.Data.UnitOfWork<Routing>())
                    {
                        var routeMain = db.Repo1
                            .Where(x => x.IdModule == route.IdModule && x.IdItem == route.IdItem &&
                                        x.IdItemType == route.IdItemType && x.IdRoutingType == RoutingType.eTypes.Main)
                            .OrderByDescending(x => x.DateChange)
                            .FirstOrDefault();

                        if (routeMain != null && route.UrlFull?.ToLower() != routeMain.UrlFull?.ToLower())
                        {
                            requestContext.HttpContext.Response.RedirectPermanent(routeMain.UrlFull, false);
                            requestContext.HttpContext.ApplicationInstance.CompleteRequest();
                        }
                    }
                }

                var module = _core.Get<ModulesManager<ApplicationCore>>().GetModule(route.IdModule);
                if (module != null)
                {
                    var arguments = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<Core.Routing.ActionArgument>>(route.Arguments, new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        Error = (s, e) => e.ErrorContext.Handled = true
                    });

                    requestContext.RouteData.Values["controller"] = module.UrlName;
                    requestContext.RouteData.Values["action"] = route.Action;
                    requestContext.RouteData.Values["url"] = requestContext.HttpContext.Request.Url?.PathAndQuery?.Replace(route.UrlFull, "");

                    if (arguments != null)
                        arguments.ForEach(x => requestContext.RouteData.Values[x.ArgumentName] = x.ArgumentValue);
                }
            }

            var routedFile = GetExistingFile(requestContext.RouteData.Values);
            if (!string.IsNullOrEmpty(routedFile))
            {
                return ((Providers.ResourceProvider)_core.Get<Core.Storage.ResourceProvider>()).GetHttpHandler(requestContext, routedFile);
            }

            var handler = base.GetHttpHandler(requestContext);
            return handler;
        }
    }

}
