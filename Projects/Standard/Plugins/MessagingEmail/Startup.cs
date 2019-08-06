using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.MessagingEmail
{
    using Core.Modules;

    class Startup : IConfigureBindings<WebApplication>
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<EMailModule>();
            bindingsCollection.SetTransient<IModuleController<EMailModule>, EMailController>();
        }
    }
}