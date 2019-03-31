using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Core.Messaging
{
    class Startup : IConfigureBindings<ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<Email.IService, Email.Service>();

            bindingsCollection.SetTransient<Email.Connectors.AmazonSES>();
            bindingsCollection.SetTransient<Email.Connectors.SmtpServer>();
        }
    }
}

