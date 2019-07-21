using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.ItemsCustomize
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplicationBase>.ConfigureBindings(IBindingsCollection<WebApplicationBase> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleItemsCustomize>();
            bindingsCollection.SetTransient<IModuleController<ModuleItemsCustomize>, ModuleController>();
        }
    }
}