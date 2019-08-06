using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.WebCoreModule
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.RegisterBindingConstraintHandler(new BindingConstraint());
            bindingsCollection.SetSingleton<WebCoreModule>();
            bindingsCollection.SetSingleton<WebCoreConfigurationChecker>();
        }
    }
}
