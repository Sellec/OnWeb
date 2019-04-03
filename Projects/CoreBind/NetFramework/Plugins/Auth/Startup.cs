using OnWeb.Core;

namespace OnWeb.Plugins.Auth
{
    using OnUtils.Architecture.AppCore;
    using OnUtils.Architecture.AppCore.DI;

    class Startup : IConfigureBindings<ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleAuth, Module2>();
        }
    }
}
