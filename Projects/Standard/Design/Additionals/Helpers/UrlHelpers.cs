using OnUtils.Utils;

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
            TypeHelper.AddAnonymousObjectToDictionary(values, new { area = OnWeb.CoreBind.Routing.AreaConstants.User });

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
            TypeHelper.AddAnonymousObjectToDictionary(values, new { area = OnWeb.CoreBind.Routing.AreaConstants.AdminPanel });

            return url.Action(action, controllerName, values);
        }

        ///// <summary>
        ///// Формирует url на основе выражения <paramref name="expression"/> для контроллера <typeparamref name="TModuleController"/>. Более подробно см. описание <see cref="Routing.Manager.CreateRoute{TModuleController}(Expression{Func{TModuleController, ActionResult}})"/>.
        ///// </summary>
        //public static string CreateRoute<TModuleController>(this UrlHelper url,  Expression<Func<TModuleController, ActionResult>> expression) where TModuleController : ModuleController
        //{
        //    return Manager.Instance.CreateRoute<TModuleController>(expression);
        //}


    }
}