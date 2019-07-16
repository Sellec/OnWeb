using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Core.Modules
{
    using Extensions.ExtensionUrl;
    using Extensions.CustomFields;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplicationBase>.ConfigureBindings(IBindingsCollection<WebApplicationBase> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleControllerTypesManager>();
            bindingsCollection.SetTransient<ExtensionUrl>();
            bindingsCollection.SetTransient<ExtensionCustomsFieldsBase>();
        }
    }
}
