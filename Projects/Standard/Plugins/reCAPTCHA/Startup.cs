using OnUtils.Architecture.AppCore;
using System.Web.Mvc;

namespace OnWeb.Plugins.reCAPTCHA
{
    class Startup : IExecuteStart
    {
        void IExecuteStart<ApplicationCore>.ExecuteStart(ApplicationCore core)
        {
            ModelValidatorProviders.Providers.Insert(0, new ModelValidatorProvider());
        }
    }
}