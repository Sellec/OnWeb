using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Materials
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleMaterials>();
            bindingsCollection.AddTransient<IModuleController<ModuleMaterials>, ModuleController>();
            bindingsCollection.AddTransient<IModuleController<ModuleMaterials>, ModuleAdminController>();
            bindingsCollection.SetTransient<MaterialsSitemapProvider>();
        }
    }
}