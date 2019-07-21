using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using OnUtils.Application.Items;

namespace OnWeb.Core
{
    class Startup : IConfigureBindings, IExecuteStart
    {
        void IConfigureBindings<WebApplicationBase>.ConfigureBindings(IBindingsCollection<WebApplicationBase> bindingsCollection)
        {
            bindingsCollection.SetSingleton<Journaling.JournalingManager>();
            bindingsCollection.SetSingleton<ServiceMonitor.Monitor>();
            bindingsCollection.SetSingleton<Users.IEntitiesManager, Users.EntitiesManager>();
            bindingsCollection.SetSingleton<Users.WebUserContextManager>();
            bindingsCollection.SetSingleton<Users.UsersManager>();
        }

        void IExecuteStart<WebApplicationBase>.ExecuteStart(WebApplicationBase core)
        {
            core.Get<ItemsManager<WebApplicationBase>>().RegisterModuleItemType<DB.User, Plugins.Customer.ModuleCustomer>();
        }
    }
}
