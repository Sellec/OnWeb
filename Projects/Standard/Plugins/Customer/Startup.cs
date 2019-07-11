using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Customer
{
    using Core.Modules;

    class Startup : IConfigureBindings, IExecuteStart
    {
        void IConfigureBindings<WebApplicationCore>.ConfigureBindings(IBindingsCollection<WebApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleCustomer, ModuleStandard>();
            bindingsCollection.SetTransient<IModuleController<ModuleCustomer>>(typeof(ModuleControllerCustomer), typeof(ModuleControllerAdminCustomer));
        }

        void IExecuteStart<WebApplicationCore>.ExecuteStart(WebApplicationCore core)
        {
            core.Get<ModuleCustomer>().RegisterExtension<Core.ModuleExtensions.CustomFields.ExtensionCustomsFieldsAdmin>();
        }
    }
}