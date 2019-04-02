using OnUtils.Application.Modules;
using OnUtils.Application.Users;
using System;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Modules
{
    /// <summary>
    /// Обязательный атрибут, без применения которого запросы к методу обрываются с исключением <see cref="InvalidOperationException"/>. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ModuleActionAttribute : AuthorizeAttribute
    {
        public ModuleActionAttribute(string alias = null, string permission = null) : this(alias, string.IsNullOrEmpty(permission) ? Guid.Empty : permission.GenerateGuid())
        {
        }

        protected ModuleActionAttribute(string alias = null, Guid? permission = null)
            : base()
        {
            Alias = alias;
            Permission = permission ?? Guid.Empty;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var r = base.AuthorizeCore(httpContext);
            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var hasPerms = true;
            Core.ApplicationCore appCore = null;

            try
            {
                if (filterContext.Controller != null)
                {
                    var controller = filterContext.Controller as ModuleControllerBase;
                    appCore = controller.ModuleBase.AppCore;

                    //todo
                    //if (Utils.TypeHelper.IsHaveBaseType(filterContext.Controller.GetType(), typeof(ModuleAdminController<>)))
                    //{
                    //    if (!controller.ModuleBase.checkPermission(ModuleCore.ACCESSADMIN)) hasPerms = false;
                    //}

                    if (hasPerms && Permission != Guid.Empty)
                    {
                        hasPerms = controller.ModuleBase.CheckPermission(controller.ModuleBase.AppCore.Get<UserContextManager<Core.ApplicationCore>>().GetCurrentUserContext(), this.Permission) == CheckPermissionResult.Allowed;
                    }
                }
            }
            catch (Exception ex)
            {
                hasPerms = false;
                Debug.WriteLine("OnAuthorization: {0}", ex.Message);
            }

            if (!hasPerms)
            {
                //UserManager.AuthorizationRedirect = filterContext.RequestContext.RouteData.Values;
                // todo UserManager.Instance.AuthorizationRedirectUrl = filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery;

                var moduleAuth = appCore?.Get<Plugins.Auth.Module>();

                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary {
                                    {"controller", moduleAuth?.UrlName ?? ""},
                                    {"action", moduleAuth != null ? "unauthorized" : ""}, // todo заменить unauthorized на ссылку на метод. Но как, если возвращаемый результат ActionResult известен только при привязке к asp.net mvc/core?
                                    {"area", Routing.AreaConstants.User}
                    });

                return;
            }

            base.OnAuthorization(filterContext);
        }

        protected override System.Web.HttpValidationStatus OnCacheAuthorization(System.Web.HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }

        #region Property
        /// <summary>
        /// Псевдоним метода. К методу можно обратиться по его имени (например, Index), но и через прописанный в свойстве псевдоним. 
        /// </summary>
        public string Alias
        {
            get;
            protected set;
        }

        /// <summary>
        /// Разрешение, требующееся для открытия данного метода. Для подробностей см. <see cref="ModuleBase{TApplication}.CheckPermission(Guid)"/>. 
        /// </summary>
        public Guid Permission
        {
            get;
            protected set;
        }

        /// <summary>
        /// Название метода для отображения в информационных сообщениях, списках методов и т.п.
        /// </summary>
        public string Caption
        {
            get;
            protected set;
        }
        #endregion
    }
}

namespace System
{
    /// <summary>
    /// </summary>
    public static class ModuleActionAttributeExtensions
    {
        /// <summary>
        /// Возвращает название метода в контроллере.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string GetDisplayName(this ActionDescriptor action)
        {
            var attr = action.GetCustomAttributes(typeof(OnWeb.CoreBind.Modules.ModuleActionAttribute), true).FirstOrDefault() as OnWeb.CoreBind.Modules.ModuleActionAttribute;
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.Caption)) return attr.Caption;
            }

            return action.ActionName;
        }
    }
}