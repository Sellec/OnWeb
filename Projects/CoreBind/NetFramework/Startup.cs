using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.CoreBind
{
    using Plugins.Auth;
    using CoreBind.Providers;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplicationCore>.ConfigureBindings(IBindingsCollection<WebApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleAuth, Module2>();
            bindingsCollection.SetSingleton<SessionBinder>();
        }
    }
}
