using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Auth
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetTransient<IModuleController<ModuleAuth>>(typeof(ModuleAuthController), typeof(ModuleAuthControllerAdmin));
        }
    }
}
