using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.CoreBind
{
    using CoreBind.Providers;
    using Plugins.Auth;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleAuth, Module2>();
            bindingsCollection.SetSingleton<SessionBinder>();
        }
    }
}
