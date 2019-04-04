using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnWeb.Plugins.reCAPTCHA
{
    class ModelValidatorProvider : System.Web.Mvc.ModelValidatorProvider
    {
        public override IEnumerable<System.Web.Mvc.ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            return new ModelValidator(metadata, context).ToEnumerable();
        }
    }
}