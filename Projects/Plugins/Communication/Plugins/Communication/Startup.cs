using OnUtils.Architecture.AppCore;

namespace OnWeb.Plugins.Communication
{
    using OnUtils.Architecture.AppCore.DI;
    using OnWeb.Core;
    using Core.Messaging.SMS;

    class Startup : IConfigureBindings<Core.ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleCommunication>();

            bindingsCollection.SetSingleton<IService, SMS.Service>();
            bindingsCollection.SetSingleton<Telegram.IService, Telegram.Service>();

            bindingsCollection.SetTransient<SMS.Connectors.AmazonSNS>();
            bindingsCollection.SetTransient<Telegram.Connectors.Bot>();
        }
    }
}
