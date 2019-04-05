using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using System.Web.Mvc;

namespace OnWeb.Plugins.reCAPTCHA
{
    class Startup : IExecuteStart, IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleReCaptcha>();
        }

        void IExecuteStart<ApplicationCore>.ExecuteStart(ApplicationCore core)
        {
            ModelValidatorProviders.Providers.Insert(0, new ModelValidatorProvider(core));
        }
    }
}