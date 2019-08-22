using System;
using System.Web.Mvc;

namespace External.ActionParameterAlias
{
    class ParameterAliasValueProvider : IValueProvider
    {
        private readonly Func<IValueProvider> _defaultValueProvider;

        private readonly ActionParameterAlias _actionParameterAlias;

        public ParameterAliasValueProvider(IValueProvider defaultValueProvider, ActionParameterAlias actionParameterAlias)
        {
            Func<IValueProvider> func = _defaultValueProvider = (() => defaultValueProvider);
            _actionParameterAlias = actionParameterAlias;
        }

        public ParameterAliasValueProvider(ControllerContext controllerContext, ActionParameterAlias actionParameterAlias)
        {
            _actionParameterAlias = actionParameterAlias;
            Func<IValueProvider> func = _defaultValueProvider = (() => controllerContext.Controller.ValueProvider);
        }

        public bool ContainsPrefix(string prefix)
        {
            if (_actionParameterAlias.ActionParameter.Equals(prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return _defaultValueProvider().ContainsPrefix(_actionParameterAlias.Alias);
            }
            return false;
        }

        public ValueProviderResult GetValue(string key)
        {
            //IL_002c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0031: Expected O, but got Unknown
            if (!_actionParameterAlias.ActionParameter.Equals(key, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return _defaultValueProvider().GetValue(_actionParameterAlias.Alias);
        }
    }
}
