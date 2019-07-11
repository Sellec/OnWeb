using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Materials
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplicationCore>.ConfigureBindings(IBindingsCollection<WebApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleMaterials>();
            bindingsCollection.AddTransient<IModuleController<ModuleMaterials>, ModuleController>();
            bindingsCollection.AddTransient<IModuleController<ModuleMaterials>, ModuleAdminController>();
            bindingsCollection.SetTransient<MaterialsSitemapProvider>();
        }
    }
}