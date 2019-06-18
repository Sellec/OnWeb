using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace External.ActionParameterAlias
{
    class ParameterAliasValueProvider : IValueProvider
    {
        private readonly ActionParameterAlias _actionParameterAlias;

        public ParameterAliasValueProvider(ActionParameterAlias actionParameterAlias)
        {
            _actionParameterAlias = actionParameterAlias;
        }

        public bool ContainsPrefix(string prefix)
        {
            if (_actionParameterAlias.ActionParameter.Equals(prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                return DefaultValueProvider.ContainsPrefix(_actionParameterAlias.Alias);
            }
            return false;
        }

        public ValueProviderResult GetValue(string key)
        {
            if (_actionParameterAlias.ActionParameter.Equals(key, StringComparison.InvariantCultureIgnoreCase))
            {
                return DefaultValueProvider.GetValue(_actionParameterAlias.Alias);
            }
            return ValueProviderResult.None;
        }

        public IValueProvider DefaultValueProvider
        {
            get;
            set;
        }

    }
}
