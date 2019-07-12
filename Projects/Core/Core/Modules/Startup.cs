using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Core.Modules
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleControllerTypesManager>();
            bindingsCollection.SetTransient<OnUtils.Application.Modules.Extensions.ExtensionUrl.ExtensionUrl, Extensions.ExtensionUrl.WebExtensionUrl>();
        }
    }
}
