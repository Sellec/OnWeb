using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Sitemap
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleSitemap>();
            bindingsCollection.AddTransient<IModuleController<ModuleSitemap>, ModuleController>();
        }
    }
}