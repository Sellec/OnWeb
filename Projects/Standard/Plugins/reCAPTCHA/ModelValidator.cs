using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.reCAPTCHA
{
    class ModelValidator : System.Web.Mvc.ModelValidator
    {
        public ModelValidator(ModelMetadata metadata, ControllerContext controllerContext) : base(metadata, controllerContext)
        {
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            throw new NotImplementedException();
            //if (Metadata.a)

            return Enumerable.Empty<ModelValidationResult>();
        }
    }
}