using OnWeb.Core;

namespace OnWeb.Plugins.ModuleAdminmain
{
    using Core.Modules;
    using OnUtils.Architecture.AppCore;
    using OnUtils.Architecture.AppCore.DI;

    class Startup : IConfigureBindings<ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<Module>();
            bindingsCollection.AddTransient<IModuleController<Module>, ModuleController>();
        }
    }
}