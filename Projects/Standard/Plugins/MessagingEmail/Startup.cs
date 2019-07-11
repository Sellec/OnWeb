using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.MessagingEmail
{
    using Core.Modules;

    class Startup : IConfigureBindings<WebApplicationCore>
    {
        void IConfigureBindings<WebApplicationCore>.ConfigureBindings(IBindingsCollection<WebApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<EMailModule>();
            bindingsCollection.SetTransient<IModuleController<EMailModule>, EMailController>();
        }
    }
}