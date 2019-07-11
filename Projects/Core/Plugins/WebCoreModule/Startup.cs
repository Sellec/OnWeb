using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.WebCoreModule
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.RegisterBindingConstraintHandler(new BindingConstraint());
            bindingsCollection.SetSingleton<WebCoreModule>();
        }
    }
}
