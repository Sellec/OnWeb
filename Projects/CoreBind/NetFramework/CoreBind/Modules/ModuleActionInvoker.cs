using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Async;

namespace OnWeb.CoreBind.Modules
{
    //using Core.Modules;

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
                actionName = nameof(Internal.ModuleControllerInternalErrors<Plugins.WebCoreModule.WebCoreModule>.PrepareError);
            }

            var action = base.FindAction(controllerContext, controllerDescriptor, actionName);

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
                actionName = nameof(Internal.ModuleControllerInternalErrors<Plugins.WebCoreModule.WebCoreModule>.PrepareError);
            }

            var action = base.FindAction(controllerContext, controllerDescriptor, actionName);

            if (action == null)
            {

            }

            return action;
        }
    }

}
