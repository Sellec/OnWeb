using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Communication
{
    class Startup : IConfigureBindings
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
