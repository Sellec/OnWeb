using OnWeb.Core;

namespace OnWeb.Plugins.ModuleAdmin
{
    using OnUtils.Architecture.AppCore;
    using OnUtils.Architecture.AppCore.DI;

    class Startup : IConfigureBindings<ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<Module>();
        }
    }
}