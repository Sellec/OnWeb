using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.MessagingEmail
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<IService, Service>();
            bindingsCollection.SetTransient<Connectors.AmazonSES>();
            bindingsCollection.SetTransient<Connectors.SmtpServer>();
        }
    }
}
