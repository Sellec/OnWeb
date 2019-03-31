using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OnWeb.Core.ModuleExtensions.CustomFields.MetadataAndValues
{
    class FieldModelMetadata : ModelMetadata
    {
        public FieldModelMetadata(ModelMetadataProvider provider, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
            : base(provider, containerType, modelAccessor, modelType, propertyName)
        {

        }

    }
}
