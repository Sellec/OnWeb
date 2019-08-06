using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using System.Web.Mvc;

namespace OnWeb.Plugins.reCAPTCHA
{
    using Core.Modules;

    class Startup : IExecuteStart, IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleReCaptcha>();
            bindingsCollection.SetTransient<IModuleController<ModuleReCaptcha>, ModuleReCaptchaController>();
        }

        void IExecuteStart<WebApplication>.ExecuteStart(WebApplication core)
        {
            ModelValidatorProviders.Providers.Insert(0, new ModelValidatorProvider(core));
        }
    }
}