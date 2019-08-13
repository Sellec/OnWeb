using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Modules.FileManager.MVC
{
    using CoreBind.Providers;

    class TraceModelMetadataProviderWithFiles : TraceModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            var additionalValues = attributes.OfType<FileDataTypeAttribute>().FirstOrDefault();

            if (additionalValues != null)
            {
                modelMetadata.AdditionalValues.Add(nameof(additionalValues.FileType), additionalValues.FileType);
            }
            return modelMetadata;
        }

    }
}