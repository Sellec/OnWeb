using OnUtils.Application.Items;
using OnUtils.Architecture.AppCore;

namespace OnWeb.Plugins.Customer
{
    using Core.DB;

    class Startup : IExecuteStart
    {
        void IExecuteStart<WebApplicationBase>.ExecuteStart(WebApplicationBase core)
        {
            core.Get<ItemsManager<WebApplicationBase>>().RegisterModuleItemType<User, ModuleCustomer>();
        }
    }
}
