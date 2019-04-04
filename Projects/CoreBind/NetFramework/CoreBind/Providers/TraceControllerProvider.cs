using OnUtils.Application.Modules;
using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace OnWeb.CoreBind.Providers
{
    using Core.Modules;
    using Routing;

    class TraceControllerProvider : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IUnitOfWorkAccessor<Core.DB.CoreContext>, IControllerFactory
    {
        private readonly IControllerFactory _controllerFactoryOld = null;

        public TraceControllerProvider(IControllerFactory controllerFactoryOld)
        {
            _controllerFactoryOld = controllerFactoryOld;
        }

        #region CoreComponentBase
        protected sealed override void OnStart()
        {
        }

        protected sealed override void OnStop()
        {
        }
        #endregion

        #region IControllerFactory
        public IController CreateController(RequestContext requestContext, string moduleName)
        {
            var isAjax = false;
            ModuleCore module = null;

            try
            {
                /*
                * Определение языка и темы
                */
                {
                    var lang = string.Format("{0}", requestContext.RouteData.Values["language"]);

                    using (var db = this.CreateUnitOfWork())
                    {
                        var query = from Language in db.Language
                                    where Language.IsDefault != 0 || Language.ShortAlias == lang
                                    orderby (Language.ShortAlias == lang ? 1 : 0) descending
                                    select Language;

                        var data = query.ToList();
                        //var sql = DB.DataContext.ExecuteQuery<DB.Language>(@"
                        //    SELECT TOP(1) *
                        //    FROM Language
                        //    WHERE IsDefault <> 0 OR ShortAlias = '" + DataManager.prepare(lang) + @"'
                        //    ORDER BY CASE WHEN ShortAlias = '" + DataManager.prepare(lang) + @"' THEN 1 ELSE 0 END DESC
                        //");
                        if (data.Count > 0)
                        {
                            var res = data.First();
                            requestContext.RouteData.Values["language"] = res.ShortAlias;
                        }
                    }
                }

                /*
                 * Ищем модуль, к которому обращаются запросом.
                 * */
                module = AppCore.Get<ModulesManager<ApplicationCore>>().GetModule(moduleName);
                if (module == null) throw new Core.Exceptions.ErrorCodeException(HttpStatusCode.NotFound, $"Адрес '{moduleName}' не найден.");

                /*
                 * Ищем контроллер, который относится к модулю.
                 * */
                var controllerType = ControllerTypeFactory.RoutingPrepareURL(requestContext.HttpContext.Request, UriExtensions.MakeRelativeFromUrl(requestContext.HttpContext.Request.Url.PathAndQuery));

                if (requestContext.RouteData.Route is Route)
                {
                    /*
                     * Анализируем адрес и устанавливаем признак, если это вызов в панель управления. Пришлось пойти на такой хак.
                     * */
                    var route = requestContext.RouteData.Route as Route;
                    if (route.Url.StartsWith("admin/madmin")) isAjax = true;

                    if (isAjax) HttpContext.Current.Items["isAjax"] = true;
                }
                
                var controller = CreateController(controllerType, module, requestContext.RouteData.Values);
                HttpContext.Current.Items["RequestContextController"] = controller;
                return controller;
            }
            catch (Exception ex)
            {
                AppCore.Get<Core.Journaling.IJournalingManager>().RegisterEvent(7, Core.Journaling.EventType.CriticalError, "123123", "12312313");
                try
                {
                    if (module == null)
                    {
                        module = new Modules.Internal.ModuleInternalErrors();
                        module.Start(AppCore);
                    }

                    var type = typeof(Modules.Internal.ModuleControllerInternalErrors<>).MakeGenericType(module.GetType());
                    var controller = CreateController(module, type, requestContext.RouteData.Values);
                    (controller as Modules.Internal.IModuleControllerInternalErrors).SetException(ex);
                    // todo (controller as Modules.ModuleController).IsAdminController = isErrorAdmin;

                    HttpContext.Current.Items["RequestContextController"] = controller;
                    return controller;
                }
                catch (Exception ex2)
                {
                    Debug.WriteLine("Throw: {0}", ex2.ToString());
                    throw ex;
                }
            }
        }

        private IController CreateController(Routing.ControllerType controllerType, ModuleCore module, RouteValueDictionary routeValues)
        {
            var targetType = module.ControllerTypes.GetValueOrDefault(controllerType.ControllerTypeID);
            if (targetType == null) throw new NotSupportedException(controllerType.ErrorCannotFindControllerTypeSpecified(module, routeValues));

            if (!controllerType.CheckPermissions(module, routeValues))
            {
                throw new Core.Exceptions.ErrorCodeException(HttpStatusCode.Forbidden, "Отсутствует доступ.");
            }

            if (targetType != null)
            {
                return CreateController(module, targetType, routeValues);
            }

            return null;
        }

        private IController CreateController(ModuleCore module, Type controllerType, RouteValueDictionary routeValues)
        {
            var controller = (Modules.ModuleControllerBase)DependencyResolver.Current.GetService(controllerType);
            if (controller == null) throw new Exception($"Контроллер для модуля '{module.UrlName}' не найден.");

            controller.Start(AppCore);

            var method = controller.GetType().GetMethod("InitController", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance);
            method.Invoke(controller, new object[] { module });

            var methods = controller.GetType().GetMethods();
            var action = routeValues["action"].ToString();
            method = (from p in methods where p.Name.ToLower() == action.ToLower() select p).FirstOrDefault();
            if (method == null)
            {
                foreach (var mm in methods)
                {
                    var attrs = mm.GetCustomAttributes(typeof(Modules.ModuleActionAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                    {
                        var attr = attrs.First() as Modules.ModuleActionAttribute;
                        if (attr.Alias == action)
                        {
                            routeValues["action"] = mm.Name;
                            method = mm;
                            break;
                        }
                    }
                }
            }

            if (method != null && routeValues["url"] != null)
            {
                var parameters = method.GetParameters();

                var url = routeValues["url"].ToString();
                // url = url.Truncate(0, url.IndexOf('?'));
                var parts = url.Split('/');
                if (parts.Length > 0)
                {
                    int idx = 0;
                    foreach (var param in parameters)
                    {
                        if (!routeValues.ContainsKey(param.Name))
                        {
                            routeValues[param.Name] = parts[idx];
                        }

                        idx++;
                        if (idx >= parts.Length) break;
                    }
                }
            }

            return controller;
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.ReadOnly;
        }

        public void ReleaseController(IController controller)
        {
            if (controller is IDisposable disposable) disposable.Dispose();
        }

        #endregion
    }

}
