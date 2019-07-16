using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Admin
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplicationBase>.ConfigureBindings(IBindingsCollection<WebApplicationBase> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleAdmin>();
        }
    }
}