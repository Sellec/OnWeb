using OnUtils.Application.Items;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Modules.Materials
{
    using Core.Modules;

    class Startup : IConfigureBindings, IExecuteStart
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleMaterials>();
            bindingsCollection.AddTransient<IModuleController<ModuleMaterials>, ModuleController>();
            bindingsCollection.AddTransient<IModuleController<ModuleMaterials>, ModuleAdminController>();
            bindingsCollection.SetTransient<MaterialsSitemapProvider>();
        }

        void IExecuteStart<WebApplication>.ExecuteStart(WebApplication core)
        {
            core.Get<ItemsManager<WebApplication>>().RegisterModuleItemType<DB.News, ModuleMaterials>();
            core.Get<ItemsManager<WebApplication>>().RegisterModuleItemType<DB.Page, ModuleMaterials>();
        }
    }
}