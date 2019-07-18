using OnUtils.Application.Items;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Materials
{
    using Core.Modules;

    class Startup : IConfigureBindings, IExecuteStart
    {
        void IConfigureBindings<WebApplicationBase>.ConfigureBindings(IBindingsCollection<WebApplicationBase> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleMaterials>();
            bindingsCollection.AddTransient<IModuleController<ModuleMaterials>, ModuleController>();
            bindingsCollection.AddTransient<IModuleController<ModuleMaterials>, ModuleAdminController>();
            bindingsCollection.SetTransient<MaterialsSitemapProvider>();
        }

        void IExecuteStart<WebApplicationBase>.ExecuteStart(WebApplicationBase core)
        {
            core.Get<ItemsManager<WebApplicationBase>>().RegisterModuleItemType<DB.News, ModuleMaterials>();
            core.Get<ItemsManager<WebApplicationBase>>().RegisterModuleItemType<DB.Page, ModuleMaterials>();
        }
    }
}