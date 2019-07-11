using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using System.Web.Mvc;

namespace OnWeb.Plugins.reCAPTCHA
{
    using Core.Modules;

    class Startup : IExecuteStart, IConfigureBindings
    {
        void IConfigureBindings<WebApplicationCore>.ConfigureBindings(IBindingsCollection<WebApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleReCaptcha>();
            bindingsCollection.SetTransient<IModuleController<ModuleReCaptcha>, ModuleReCaptchaController>();
        }

        void IExecuteStart<WebApplicationCore>.ExecuteStart(WebApplicationCore core)
        {
            ModelValidatorProviders.Providers.Insert(0, new ModelValidatorProvider(core));
        }
    }
}