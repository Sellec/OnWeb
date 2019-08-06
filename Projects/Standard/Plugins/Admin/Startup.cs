using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Admin
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleAdmin, ModuleStandard>();
            bindingsCollection.SetTransient<IModuleController<ModuleAdmin>, ModuleAdminController>();
        }
    }
}
