using Microsoft.AspNetCore.Mvc.ModelBinding;
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
}
