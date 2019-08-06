using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Register
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleRegister>();
            bindingsCollection.SetTransient<IModuleController<ModuleRegister>, ModuleRegisterController>();
        }
    }
}
