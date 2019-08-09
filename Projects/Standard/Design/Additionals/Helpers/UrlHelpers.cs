using OnUtils.Utils;
using OnWeb.Core.Modules;
using OnWeb.Core.Modules;
using OnWeb.CoreBind.Routing;
using System.Linq;
using System.Linq.Expressions;

namespace System.Web.Mvc
{
    /// <summary>
    /// </summary>
    public static class UrlHelpers
    {
        /// <summary>
        /// Проверяет, является ли адрес <paramref name="url"/> текущим открытым адресом. 
        /// </summary>
        public static bool IsCurrent(this UrlHelper helper, string url)
        {
            var d = helper.RequestContext.HttpContext.Request.RawUrl;
            if (url == d) return true;

            return false;
        }

        /// <summary>
        /// Преобразует переданный относительный путь в абсолютный с указанием имени сервера.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="virtualPath">Относительный путь</param>
        /// <param name="_args"></param>
        /// <returns></returns>
        public static string ContentFullPath(this UrlHelper url, string virtualPath = "", params object[] _args)
        {
            try
            {
                Uri test;
                if (Uri.TryCreate(virtualPath, UriKind.Absolute, out test)) virtualPath = test.PathAndQuery;

                var result = string.Empty;
                Uri requestUrl = url.RequestContext.HttpContext.Request.Url;

                var path = string.IsNullOrEmpty(virtualPath) ? "~" : virtualPath;
                if (_args != null && _args.Length > 0) path = string.Format(path, _args);

                result = string.Format("{0}://{1}{2}",
                                       requestUrl.Scheme,
                                       requestUrl.Authority,
                                       VirtualPathUtility.ToAbsolute(path));
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }
        }

        /// <summary>
        /// См. <see cref="ContentFullPath(UrlHelper, Uri)"/>.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        public static string ContentFullPath(this UrlHelper url, Uri virtualPath = null)
        {
            return ContentFullPath(url, virtualPath?.ToString());
        }

        /// <summary>
        /// См. <see cref="UrlHelper.Action(string, string)"/>. Генерирует адрес для зоны "пользовательская".
        /// </summary>
        public static string ActionUser(this UrlHelper url, string action, string controllerName)
        {
            return url.ActionUser(action, controllerName, null);
        }

        /// <summary>
        /// См. <see cref="UrlHelper.Action(string, string, object)"/>. Генерирует адрес для зоны "пользовательская".
        /// </summary>
        public static string ActionUser(this UrlHelper url, string action, string controllerName, object routeValues)
        {
            var values = TypeHelper.ObjectToDictionary(routeValues);
            TypeHelper.AddAnonymousObjectToDictionary(values, new { area = AreaConstants.User });

            return url.Action(action, controllerName, values);
        }

        /// <summary>
        /// См. <see cref="UrlHelper.Action(string, string)"/>. Генерирует адрес для зоны "панель управления".
        /// </summary>
        public static string ActionAdmin(this UrlHelper url, string action, string controllerName)
        {
            return url.ActionAdmin(action, controllerName, null);
        }

        /// <summary>
        /// См. <see cref="UrlHelper.Action(string, string, object)"/>. Генерирует адрес для зоны "панель управления".
        /// </summary>
        public static string ActionAdmin(this UrlHelper url, string action, string controllerName, object routeValues)
        {
            var values = TypeHelper.ObjectToDictionary(routeValues);
            TypeHelper.AddAnonymousObjectToDictionary(values, new { area = AreaConstants.AdminPanel });

            return url.Action(action, controllerName, values);
        }

        /// <summary>
        /// Формирует абсолютный или относительный url (см. <paramref name="includeAuthority"/>) к разделу по-умолчанию в пользовательской части сайта для контроллера <typeparamref name="TModuleController"/>.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="includeAuthority">Если равно true, то формируется абсолютный url, включающий в себя адрес сервера.</param>
        /// <seealso cref="RoutingManager.CreateRoute{TModule, TModuleController}(Expression{Func{TModuleController, ActionResult}}, bool)"/>
        public static Uri CreateRoute(this UrlHelper helper, bool includeAuthority = false)
        {
            var appCore = helper.RequestContext.HttpContext.GetAppCore();
            var module = appCore.GetModulesManager().GetModule(appCore.WebConfig.IdModuleDefault);
            if (module == null) return null;

            var controllerTypes = appCore.Get<ModuleControllerTypesManager>().GetModuleControllerTypes(module.QueryType);
            var controllerType = controllerTypes.Where(x => x.Key == ControllerTypeDefault.TypeID).Select(x => x.Value).FirstOrDefault();
            if (controllerType == null) return null;

            var methodInfo = typeof(UrlHelpers).GetMethods(Reflection.BindingFlags.Public | Reflection.BindingFlags.Static).Where(x => x.IsGenericMethod && x.Name == nameof(CreateRoute) && x.GetParameters().Length == 2).FirstOrDefault();
            if (methodInfo == null) return null;

            return (Uri)methodInfo.MakeGenericMethod(module.QueryType, controllerType).Invoke(null, new object[] { helper, includeAuthority });
        }


        /// <summary>
        /// Формирует абсолютный или относительный url (см. <paramref name="includeAuthority"/>) к разделу по-умолчанию в пользовательской части сайта для контроллера <typeparamref name="TModuleController"/>.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="includeAuthority">Если равно true, то формируется абсолютный url, включающий в себя адрес сервера.</param>
        /// <seealso cref="RoutingManager.CreateRoute{TModule, TModuleController}(Expression{Func{TModuleController, ActionResult}}, bool)"/>
        public static Uri CreateRoute<TModule, TModuleController>(this UrlHelper helper, bool includeAuthority = false)
            where TModule : ModuleCore<TModule>
            where TModuleController : ModuleControllerUser<TModule>, IModuleController<TModule>
        {
            return helper.CreateRoute<TModule, TModuleController>(x => x.Index());
        }

        /// <summary>
        /// Формирует абсолютный или относительный url (см. <paramref name="includeAuthority"/>) на основе выражения <paramref name="expression"/> для контроллера <typeparamref name="TModuleController"/>.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="expression">Выражение, содержащее вызов метода контроллера, к которому следует построить маршрут. Все аргументы вызываемого метода должны быть указаны. Если аргумент указывается как null, то он игнорируется. Если аргумент задан явно, то он передается в адресе.</param>
        /// <param name="includeAuthority">Если равно true, то формируется абсолютный url, включающий в себя адрес сервера.</param>
        /// <seealso cref="RoutingManager.CreateRoute{TModule, TModuleController}(Expression{Func{TModuleController, ActionResult}}, bool)"/>
        public static Uri CreateRoute<TModule, TModuleController>(this UrlHelper helper, Expression<Func<TModuleController, ActionResult>> expression, bool includeAuthority = false)
            where TModule : ModuleCore<TModule>
            where TModuleController : IModuleController<TModule>
        {
            return helper.RequestContext.HttpContext.GetAppCore().Get<RoutingManager>().CreateRoute<TModule, TModuleController>(expression);
        }
    }
}