using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.MessagingEmail
{
    using Core.Modules;

    class Startup : IConfigureBindings<ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<EMailModule>();
            bindingsCollection.SetTransient<IModuleController<EMailModule>, EMailController>();
        }
    }
}