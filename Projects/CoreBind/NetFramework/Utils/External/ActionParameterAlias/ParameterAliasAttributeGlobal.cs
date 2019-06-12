using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Reflection;
using System;

namespace External.ActionParameterAlias
{
    class ParameterAliasAttributeGlobal : AuthorizeAttribute
    {
        public ParameterAliasAttributeGlobal()
        {
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var attribute = filterContext.ActionDescriptor.
                GetCustomAttributes(typeof(ParameterAliasAttribute), false).
                OfType<ParameterAliasAttribute>().
                FirstOrDefault();

            if (attribute != null)
            {
                var valueProviderSource = filterContext.Controller.ValueProvider;
                var valueProviderNew = new ValueProviderCollection
                {
                    new ParameterAliasValueProvider(valueProviderSource, new ActionParameterAlias(attribute.ParameterName, attribute.AliasName)),
                    valueProviderSource
                };
                filterContext.Controller.ValueProvider = valueProviderNew;
            }

            {
                var list = new List<ActionParameterAlias>();
                var parameters = filterContext.ActionDescriptor.GetParameters().Where(x=>x.GetCustomAttributes(typeof(AliasAttribute), false).Count() > 0).ToList();
                AddAliases(list, parameters);
                var valueProviderSource = filterContext.Controller.ValueProvider;
                var valueProviderNew = new ValueProviderCollection();
                foreach (var item in list)
                {
                    valueProviderNew.Add(new ParameterAliasValueProvider(valueProviderSource, item));
                }
                valueProviderNew.Add(valueProviderSource);
                filterContext.Controller.ValueProvider = valueProviderNew;
            }
        }

        private static void AddAliases(List<ActionParameterAlias> parameterAliases, IEnumerable<ParameterDescriptor> parameterDescriptors)
        {
            foreach (var parameterDescriptor in parameterDescriptors)
            {
                AddAlias(parameterAliases, parameterDescriptor, parameterDescriptor.ParameterType, parameterDescriptor.ParameterName);
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
