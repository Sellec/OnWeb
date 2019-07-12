using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Core
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<Routing.UrlManager>();
            bindingsCollection.SetSingleton<ServiceMonitor.Monitor>();
            bindingsCollection.SetSingleton<Users.IEntitiesManager, Users.EntitiesManager>();
            bindingsCollection.SetSingleton<Users.WebUserContextManager>();
            bindingsCollection.SetSingleton<Users.UsersManager>();
        }
    }
}
