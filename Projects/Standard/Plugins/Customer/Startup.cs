using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.Customer
{
    using Core.Modules;

    class Startup : IConfigureBindings, IExecuteStart
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleCustomer, ModuleStandard>();
            bindingsCollection.SetTransient<IModuleController<ModuleCustomer>>(typeof(ModuleControllerCustomer), typeof(ModuleControllerAdminCustomer));
        }

        void IExecuteStart<ApplicationCore>.ExecuteStart(WebApplicationBase core)
        {
            core.Get<ModuleCustomer>().RegisterExtension<Core.ModuleExtensions.CustomFields.ExtensionCustomsFieldsAdmin>();
        }
    }
}