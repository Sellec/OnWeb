using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Default
{
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetTransient<IModuleController<ModuleDefault>, ModuleDefaultController>();
        }
    }
}