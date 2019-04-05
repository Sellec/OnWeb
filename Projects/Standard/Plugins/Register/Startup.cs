using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Register
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleRegister>();
            bindingsCollection.SetTransient<IModuleController<ModuleRegister>, ModuleRegisterController>();
        }
    }
}
