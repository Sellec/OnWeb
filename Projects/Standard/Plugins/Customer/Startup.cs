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
        void IConfigureBindings<WebApplicationBase>.ConfigureBindings(IBindingsCollection<WebApplicationBase> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleCustomer, ModuleStandard>();
            bindingsCollection.SetTransient<IModuleController<ModuleCustomer>>(typeof(ModuleControllerCustomer), typeof(ModuleControllerAdminCustomer));
        }

        void IExecuteStart<WebApplicationBase>.ExecuteStart(WebApplicationBase core)
        {
            core.Get<ItemsManager<WebApplicationBase>>().RegisterModuleItemType<ProfileEdit, ModuleCustomer>();
            core.Get<ItemsManager<WebApplicationBase>>().RegisterModuleItemType<PreparedForRegister, ModuleCustomer>();
            core.Get<ItemsManager<WebApplicationBase>>().RegisterModuleItemType<Register, ModuleCustomer>();
        }
    }
}