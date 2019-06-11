using System.Linq;
using System.Web.Mvc;

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
                    valueProviderSource,
                    new ParameterAliasValueProvider(valueProviderSource, new ActionParameterAlias(attribute.ParameterName, attribute.AliasName))
                };
                filterContext.Controller.ValueProvider = valueProviderNew;
            }
        }
    }
}
