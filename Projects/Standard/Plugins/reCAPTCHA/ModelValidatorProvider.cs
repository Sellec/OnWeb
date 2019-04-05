using OnUtils.Architecture.AppCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnWeb.Plugins.reCAPTCHA
{
    class ModelValidatorProvider : System.Web.Mvc.ModelValidatorProvider
    {
        private readonly ApplicationCore _appCore;

        public ModelValidatorProvider(ApplicationCore appCore)
        {
            _appCore = appCore;
        }

        public override IEnumerable<System.Web.Mvc.ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            if (_appCore.GetState() != CoreComponentState.Started) return Enumerable.Empty<System.Web.Mvc.ModelValidator>();
            var module = _appCore.Get<ModuleReCaptcha>();
            var config = module.GetConfiguration<ModuleReCaptchaConfiguration>();

            return config.IsEnabledValidation ? new ModelValidator(config.PrivateKey, metadata, context).ToEnumerable() : Enumerable.Empty<System.Web.Mvc.ModelValidator>();
        }
    }
}
