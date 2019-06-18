using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace External.ActionParameterAlias
{
    class ParameterAliasAttributeGlobal : IResourceFilter
    {
        public ParameterAliasAttributeGlobal()
        {
        }

        void IResourceFilter.OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        void IResourceFilter.OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)) return;

            var attribute = actionDescriptor.
                MethodInfo.
                GetCustomAttributes(typeof(ParameterAliasAttribute), false).
                OfType<ParameterAliasAttribute>().
                FirstOrDefault();

            var providers = new List<ParameterAliasValueProvider>();

            if (attribute != null)
            {
                providers.Add(new ParameterAliasValueProvider(new ActionParameterAlias(attribute.ParameterName, attribute.AliasName)));
            }

            {
                var list = new List<ActionParameterAlias>();
                var parameters = actionDescriptor.MethodInfo.GetParameters().Where(x => x.GetCustomAttributes(typeof(AliasAttribute), false).Count() > 0).ToList();
                AddAliases(list, parameters);
                foreach (var item in list)
                {
                    providers.Add(new ParameterAliasValueProvider(item));
                }
            }

            if (providers.Count > 0)
            {
                context.ValueProviderFactories.Add(new ParameterAliasValueProviderFactory(providers.ToArray()));
            }
        }

        private static void AddAliases(List<ActionParameterAlias> parameterAliases, IEnumerable<ParameterInfo> parameterDescriptors)
        {
            foreach (var parameterDescriptor in parameterDescriptors)
            {
                AddAlias(parameterAliases, parameterDescriptor, parameterDescriptor.ParameterType, parameterDescriptor.Name);
            }
        }

        private static void AddAlias(List<ActionParameterAlias> parameterAliases, ICustomAttributeProvider propertyInfo, Type propertyType, string propertyName)
        {
            var array = (AliasAttribute[])propertyInfo.GetCustomAttributes(typeof(AliasAttribute), false);
            if (array.Any())
            {
                foreach (var alias in array)
                {
                    parameterAliases.Add(new ActionParameterAlias(propertyName, alias.Name));
                }
            }
            var properties = propertyType.GetProperties();
            AddAliases(parameterAliases, properties);
        }

        private static void AddAliases(List<ActionParameterAlias> parameterAliases, IEnumerable<PropertyInfo> incomingPropertyInfos)
        {
            foreach (var incomingPropertyInfo in incomingPropertyInfos)
            {
                AddAlias(parameterAliases, incomingPropertyInfo, incomingPropertyInfo.PropertyType, incomingPropertyInfo.Name);
            }
        }

    }
}
