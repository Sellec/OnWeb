using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Modules
{
    class CustomReflectedActionDescriptor : ReflectedActionDescriptor
    {
        public CustomReflectedActionDescriptor(MethodInfo methodInfo, string actionName, ControllerDescriptor controllerDescriptor) :
            base(methodInfo, actionName, controllerDescriptor)
        {

        }

        public IDictionary<string, CustomReflectedActionParameter> args;

        public override object Execute(ControllerContext controllerContext, IDictionary<string, object> parameters)
        {
            if (args != null)
                foreach (var pair in args)
                    if (parameters.ContainsKey(pair.Key) && pair.Value.HasDefaultValue)
                        parameters[pair.Key] = pair.Value.Value;

            return base.Execute(controllerContext, parameters);
        }
    }

    class CustomReflectedActionParameter
    {
        public bool HasDefaultValue { get; set; }

        public object Value { get; set; }
    }
}
