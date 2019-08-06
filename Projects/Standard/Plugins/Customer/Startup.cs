using OnUtils.Application.Items;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Customer
{
    using Core.Modules;
    using Model;
    using Register.Model;

    class Startup : IConfigureBindings, IExecuteStart
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleCustomer, ModuleStandard>();
            bindingsCollection.SetTransient<IModuleController<ModuleCustomer>>(typeof(ModuleControllerCustomer), typeof(ModuleControllerAdminCustomer));
        }

        void IExecuteStart<WebApplication>.ExecuteStart(WebApplication core)
        {
            core.Get<ItemsManager<WebApplication>>().RegisterModuleItemType<ProfileEdit, ModuleCustomer>();
            core.Get<ItemsManager<WebApplication>>().RegisterModuleItemType<PreparedForRegister, ModuleCustomer>();
            core.Get<ItemsManager<WebApplication>>().RegisterModuleItemType<Register, ModuleCustomer>();
        }
    }
}