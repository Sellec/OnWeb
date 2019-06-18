using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace External.ActionParameterAlias
{
    class ParameterAliasValueProviderFactory : IValueProviderFactory
    {
        private ParameterAliasValueProvider[] _providers;

        public ParameterAliasValueProviderFactory(ParameterAliasValueProvider[] providers)
        {
            _providers = providers ?? new ParameterAliasValueProvider[] { };
        }

        Task IValueProviderFactory.CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context.ValueProviders is List<IValueProvider> list)
            {
                var compositeProvider = new CompositeValueProvider(list.ToArray());
                _providers.ForEach(x => x.DefaultValueProvider = compositeProvider);
                list.Clear();
                list.AddRange(_providers);
                list.Add(compositeProvider);
            }
            return Task.CompletedTask;
        }
    }

    class ParameterAliasValueProvider : IValueProvider
    {
        private readonly ActionParameterAlias _actionParameterAlias;

        public ParameterAliasValueProvider(ActionParameterAlias actionParameterAlias)
        {
            _actionParameterAlias = actionParameterAlias;
        }

        //public ParameterAliasValueProvider(ControllerContext controllerContext, ActionParameterAlias actionParameterAlias)
        //{
        //    _actionParameterAlias = actionParameterAlias;
        //    Func<IValueProvider> func = _defaultValueProvider = (() => controllerContext.Controller.ValueProvider);
        //}

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
