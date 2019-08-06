using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Core.Messaging
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ISubscriptionsManager, SubscriptionsManager>();
        }
    }
}

