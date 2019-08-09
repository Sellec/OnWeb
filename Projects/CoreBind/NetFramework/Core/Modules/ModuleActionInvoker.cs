using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Linq;
using System.Web.Mvc.Async;

namespace OnWeb.Core.Modules
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
            var controllerType = controllerDescriptor.ControllerType;
            var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            var actionMethods = Array.FindAll<MethodInfo>(methods, new Predicate<MethodInfo>(IsValidActionMethod));
            var standardRouteMethods = new HashSet<MethodInfo>(actionMethods);

            if (typeof(Internal.IModuleControllerInternalErrors).IsAssignableFrom(controllerDescriptor.ControllerType))
            {
                actionName = nameof(Internal.ModuleControllerInternalErrors<OnWeb.Modules.WebCoreModule.WebCoreModule>.PrepareError);
            }

            var action = base.FindAction(controllerContext, controllerDescriptor, actionName);

            if (action == null)
            {
                var actionMethodCandidate = actionMethods.FirstOrDefault(x => x.Name == actionName);
                if (actionMethodCandidate != null)
                {
                    var parametersValues = actionMethodCandidate.GetParameters().ToDictionary(x => x.Name, x => new CustomReflectedActionParameter() { HasDefaultValue = x.HasDefaultValue, Value = x.HasDefaultValue ? x.DefaultValue : null });
                    action = new ReflectedActionDescriptor(actionMethodCandidate, actionName, controllerDescriptor);
                }
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
            var controllerType = controllerDescriptor.ControllerType;
            var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            var actionMethods = Array.FindAll<MethodInfo>(methods, new Predicate<MethodInfo>(IsValidActionMethod));
            var standardRouteMethods = new HashSet<MethodInfo>(actionMethods);

            if (typeof(Internal.IModuleControllerInternalErrors).IsAssignableFrom(controllerDescriptor.ControllerType))
            {
                actionName = nameof(Internal.ModuleControllerInternalErrors<OnWeb.Modules.WebCoreModule.WebCoreModule>.PrepareError);
            }

            var action = base.FindAction(controllerContext, controllerDescriptor, actionName);

            if (action == null)
            {
                var actionMethodCandidate = actionMethods.FirstOrDefault(x => x.Name == actionName);
                if (actionMethodCandidate != null)
                {
                    var parametersValues = actionMethodCandidate.GetParameters().ToDictionary(x => x.Name, x => new CustomReflectedActionParameter() { HasDefaultValue = x.HasDefaultValue, Value = x.HasDefaultValue ? x.DefaultValue : null });
                    action = new ReflectedActionDescriptor(actionMethodCandidate, actionName, controllerDescriptor);
                }
            }

            return action;
        }
    }

}
