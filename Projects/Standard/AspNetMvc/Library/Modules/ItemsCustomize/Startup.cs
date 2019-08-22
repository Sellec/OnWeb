using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Modules.ItemsCustomize
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleItemsCustomize>();
            bindingsCollection.SetTransient<IModuleController<ModuleItemsCustomize>, ModuleController>();
        }
    }
}