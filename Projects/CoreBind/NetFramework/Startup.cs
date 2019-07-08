using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.CoreBind
{
    using Plugins.Auth;
    using CoreBind.Providers;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleAuth, Module2>();
            bindingsCollection.SetSingleton<SessionBinder>();
        }
    }
}
