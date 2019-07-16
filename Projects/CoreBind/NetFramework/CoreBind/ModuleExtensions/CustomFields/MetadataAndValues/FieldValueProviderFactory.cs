using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OnWeb.Core.Modules.Extensions.CustomFields.MetadataAndValues
{
    class FieldValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new FieldValueProvider(controllerContext);
        }
    }
}
