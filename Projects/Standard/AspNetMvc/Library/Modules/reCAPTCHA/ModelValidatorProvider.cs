using OnUtils.Architecture.AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Modules.reCAPTCHA
{
    class ModelValidatorProvider : System.Web.Mvc.ModelValidatorProvider
    {
        private readonly WebApplication _appCore;

        public ModelValidatorProvider(WebApplication appCore)
        {
            _appCore = appCore;
        }

        public override IEnumerable<System.Web.Mvc.ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            if (_appCore.GetState() != CoreComponentState.Started) return Enumerable.Empty<System.Web.Mvc.ModelValidator>();
            var module = _appCore.Get<ModuleReCaptcha>();
            var cfg = module.GetConfiguration<ModuleReCaptchaConfiguration>();

            return cfg.IsEnabledValidation && !string.IsNullOrEmpty(cfg.PublicKey) && !string.IsNullOrEmpty(cfg.PrivateKey) ?
                new ModelValidator(_appCore, cfg.PrivateKey, metadata, context).ToEnumerable() :
                Enumerable.Empty<System.Web.Mvc.ModelValidator>();
        }
    }
}
