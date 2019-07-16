using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Customer
{
    using Core.Modules;

    class Startup : IConfigureBindings, IExecuteStart
    {
        void IConfigureBindings<WebApplicationBase>.ConfigureBindings(IBindingsCollection<WebApplicationBase> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleCustomer, ModuleStandard>();
            bindingsCollection.SetTransient<IModuleController<ModuleCustomer>>(typeof(ModuleControllerCustomer), typeof(ModuleControllerAdminCustomer));
        }

        void IExecuteStart<WebApplicationBase>.ExecuteStart(WebApplicationBase core)
        {
            core.Get<ModuleCustomer>().RegisterExtension<Core.ModuleExtensions.CustomFields.ExtensionCustomsFieldsAdmin>();
        }
    }
}