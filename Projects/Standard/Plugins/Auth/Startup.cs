using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Auth
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetTransient<IModuleController<ModuleAuth>>(typeof(ModuleAuthController), typeof(ModuleAuthControllerAdmin));
        }
    }
}
