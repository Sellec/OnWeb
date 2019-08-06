using OnUtils.Application.Items;
using OnUtils.Architecture.AppCore;

namespace OnWeb.Plugins.Customer
{
    using Core.DB;

    class Startup : IExecuteStart
    {
        void IExecuteStart<WebApplication>.ExecuteStart(WebApplication core)
        {
            core.Get<ItemsManager<WebApplication>>().RegisterModuleItemType<User, ModuleCustomer>();
        }
    }
}
