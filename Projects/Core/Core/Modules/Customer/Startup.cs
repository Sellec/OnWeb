using OnUtils.Application.Items;
using OnUtils.Architecture.AppCore;

namespace OnWeb.Modules.Customer
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
