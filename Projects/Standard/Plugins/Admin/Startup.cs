using OnWeb.Core;

namespace OnWeb.Plugins.Admin
{
    using Core.Modules;
    using OnUtils.Architecture.AppCore;
    using OnUtils.Architecture.AppCore.DI;

    class Startup : IConfigureBindings<ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<Module, ModuleStandard>();
            bindingsCollection.SetTransient<IModuleController<Module>, ModuleController>();
        }
    }
}
