using System;
using System.Web.Mvc;

namespace OnWeb.Core.Modules.Extensions.CustomFields.MetadataAndValues
{
    class FieldModelMetadata : ModelMetadata
    {
        public FieldModelMetadata(ModelMetadataProvider provider, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
            : base(provider, containerType, modelAccessor, modelType, propertyName)
        {

        }

    }
}
