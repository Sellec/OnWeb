using OnUtils.Application.Modules;
using OnUtils.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace OnWeb.CoreBind.Modules
{
    using Core.Modules;

    class ModuleActionAsyncInvoker : AsyncControllerActionInvoker
    {
        private ModuleControllerBase _controller = null;

        public ModuleActionAsyncInvoker(ModuleControllerBase controller)
        {
            _controller = controller;
        }

        protected bool IsValidActionMethod(MethodInfo methodInfo)
        {
            return !methodInfo.IsSpecialName && !methodInfo.GetBaseDefinition().DeclaringType.IsAssignableFrom(typeof(Controller));
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName)
        {
            var ControllerType = controllerDescriptor.ControllerType;
            MethodInfo[] methods = ControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
            var ActionMethods = Array.FindAll<MethodInfo>(methods, new Predicate<MethodInfo>(this.IsValidActionMethod));
            var StandardRouteMethods = new HashSet<MethodInfo>(ActionMethods);

            if (typeof(Internal.IModuleControllerInternalErrors).IsAssignableFrom(controllerDescriptor.ControllerType))
            {
                actionName = nameof(Internal.ModuleControllerInternalErrors<Plugins.CoreModule.CoreModule>.PrepareError);
            }

            var action = base.FindAction(controllerContext, controllerDescriptor, actionName);
            if (action == null)
            {
                try
                {
                    var module = (ModuleCore)_controller.ModuleBase;
                    var extensions = module.GetExtensions();
                    foreach (var extension in extensions)
                    {
                        var method = extension.GetType().GetMethod(actionName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (method == null)
                        {
                            method = (from p in extension.GetType().GetMethods()
                                      where Attribute.IsDefined(p, typeof(ModuleActionAttribute)) &&
                                            string.Equals(p.GetCustomAttributes<ModuleActionAttribute>(true).First().Alias, actionName, StringComparison.InvariantCultureIgnoreCase)
                                      select p).FirstOrDefault();
                        }

                        if (method != null)
                        {
                            var attrs = method.GetCustomAttributes(true);
                            if (extension.Attributes.IsAdminPart && _controller.ModuleBase.CheckPermission(module.AppCore.Get<UserContextManager<ApplicationCore>>().GetCurrentUserContext(), ModulesConstants.PermissionManage) != CheckPermissionResult.Allowed)
                                throw new Exception("Недостаточно прав!");

                            MethodInfo mm = null;

                            var parametersValuesSource = method.GetParameters().Select(x => x.HasDefaultValue ? x.DefaultValue : null).ToArray();

                            var url = controllerContext.RouteData.Values["url"];
                            var parts = (url != null ? url.ToString() : "").Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < parts.Length && i < parametersValuesSource.Length; i++)
                                parametersValuesSource[i] = parts[i];

                            var parametersTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                            if (parametersTypes.Length == 0)
                                mm = controllerContext.Controller.GetType().GetMethod("ExtensionWrapper_0", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (parametersTypes.Length > 0)
                                mm = controllerContext.Controller.GetType()
                                            .GetMethod("ExtensionWrapper_" + parametersTypes.Length, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                            .MakeGenericMethod(parametersTypes);

                            var parametersValues = mm.GetParameters().ToDictionary(x => x.Name, x => new CustomReflectedActionParameter() { HasDefaultValue = x.HasDefaultValue, Value = x.HasDefaultValue ? x.DefaultValue : null });
                            parametersValues["_extension"] = new CustomReflectedActionParameter() { HasDefaultValue = true, Value = extension };
                            parametersValues["_extensionMethod"] = new CustomReflectedActionParameter() { HasDefaultValue = true, Value = method };

                            for (int i = 0; i < parametersValuesSource.Length; i++)
                                if (parametersValuesSource[i] != null)
                                {
                                    var type = parametersTypes[i];
                                    parametersValues[parametersValues.ElementAt(i + 2).Key].Value = Convert.ChangeType(parametersValuesSource[i], type);
                                    parametersValues[parametersValues.ElementAt(i + 2).Key].HasDefaultValue = true;
                                }

                            extension.ControllerBase = controllerContext.Controller as ModuleControllerBase;
                            action = new CustomReflectedActionDescriptor(mm, actionName, controllerDescriptor)
                            {
                                args = parametersValues
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Ошибка во время поиска метода расширения.", ex);
                }
            }

            if (action == null)
            {

            }

            return action;
        }
    }

    class ModuleActionInvoker : ControllerActionInvoker
    {
        private ModuleControllerBase _controller = null;

        public ModuleActionInvoker(ModuleControllerBase controller)
        {
            _controller = controller;
        }

        protected bool IsValidActionMethod(MethodInfo methodInfo)
        {
            return !methodInfo.IsSpecialName && !methodInfo.GetBaseDefinition().DeclaringType.IsAssignableFrom(typeof(Controller));
        }

        protected override ActionDescriptor FindAction(ControllerContext controllerContext, ControllerDescriptor controllerDescriptor, string actionName)
        {
            var ControllerType = controllerDescriptor.ControllerType;
            MethodInfo[] methods = ControllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod);
            var ActionMethods = Array.FindAll<MethodInfo>(methods, new Predicate<MethodInfo>(this.IsValidActionMethod));
            var StandardRouteMethods = new HashSet<MethodInfo>(ActionMethods);

            if (typeof(Internal.IModuleControllerInternalErrors).IsAssignableFrom(controllerDescriptor.ControllerType))
            {
                actionName = nameof(Internal.ModuleControllerInternalErrors<Plugins.CoreModule.CoreModule>.PrepareError);
            }

            var action = base.FindAction(controllerContext, controllerDescriptor, actionName);
            if (action == null)
            {
                try
                {
                    var module = (ModuleCore)_controller.ModuleBase;
                    var extensions = module.GetExtensions();
                    foreach (var extension in extensions)
                    {
                        var method = extension.GetType().GetMethod(actionName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (method == null)
                        {
                            method = (from p in extension.GetType().GetMethods()
                                      where Attribute.IsDefined(p, typeof(ModuleActionAttribute)) &&
                                            string.Equals(p.GetCustomAttributes<ModuleActionAttribute>(true).First().Alias, actionName, StringComparison.InvariantCultureIgnoreCase)
                                      select p).FirstOrDefault();
                        }

                        if (method != null)
                        {
                            var attrs = method.GetCustomAttributes(true);
                            if (extension.Attributes.IsAdminPart && _controller.ModuleBase.CheckPermission(module.AppCore.Get<UserContextManager<ApplicationCore>>().GetCurrentUserContext(), ModulesConstants.PermissionManage) != CheckPermissionResult.Allowed)
                                throw new Exception("Недостаточно прав!");

                            MethodInfo mm = null;

                            var parametersValuesSource = method.GetParameters().Select(x => x.HasDefaultValue ? x.DefaultValue : null).ToArray();

                            var url = controllerContext.RouteData.Values["url"];
                            var parts = (url != null ? url.ToString() : "").Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                            for (int i = 0; i < parts.Length && i < parametersValuesSource.Length; i++)
                                parametersValuesSource[i] = parts[i];

                            var parametersTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                            if (parametersTypes.Length == 0)
                                mm = controllerContext.Controller.GetType().GetMethod("ExtensionWrapper_0", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (parametersTypes.Length > 0)
                                mm = controllerContext.Controller.GetType()
                                            .GetMethod("ExtensionWrapper_" + parametersTypes.Length, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                            .MakeGenericMethod(parametersTypes);

                            var parametersValues = mm.GetParameters().ToDictionary(x => x.Name, x => new CustomReflectedActionParameter() { HasDefaultValue = x.HasDefaultValue, Value = x.HasDefaultValue ? x.DefaultValue : null });
                            parametersValues["_extension"] = new CustomReflectedActionParameter() { HasDefaultValue = true, Value = extension };
                            parametersValues["_extensionMethod"] = new CustomReflectedActionParameter() { HasDefaultValue = true, Value = method };

                            for (int i = 0; i < parametersValuesSource.Length; i++)
                                if (parametersValuesSource[i] != null)
                                {
                                    var type = parametersTypes[i];
                                    parametersValues[parametersValues.ElementAt(i + 2).Key].Value = Convert.ChangeType(parametersValuesSource[i], type);
                                    parametersValues[parametersValues.ElementAt(i + 2).Key].HasDefaultValue = true;
                                }

                            extension.ModelStateBase = (controllerContext.Controller as Controller)?.ModelState;
                            extension.ControllerBase = controllerContext.Controller as ModuleControllerBase;
                            action = new CustomReflectedActionDescriptor(mm, actionName, controllerDescriptor)
                            {
                                args = parametersValues
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Ошибка во время поиска метода расширения.", ex);
                }
            }

            if (action == null)
            {

            }

            return action;
        }
    }

}
