using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;

namespace OnWeb.CoreBind.Providers
{
    class TraceModelMultipleMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private List<DataAnnotationsModelMetadataProvider> _providers = new List<DataAnnotationsModelMetadataProvider>();

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {

            return base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
        }

        protected override IEnumerable<Attribute> FilterAttributes(Type containerType, PropertyDescriptor propertyDescriptor, IEnumerable<Attribute> attributes)
        {
            return base.FilterAttributes(containerType, propertyDescriptor, attributes);
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            return base.GetMetadataForProperties(container, containerType);
        }

        protected override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, PropertyDescriptor propertyDescriptor)
        {
            return base.GetMetadataForProperty(modelAccessor, containerType, propertyDescriptor);
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            return base.GetMetadataForProperty(modelAccessor, containerType, propertyName);
        }

        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            return base.GetMetadataForType(modelAccessor, modelType);
        }

        protected override ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return base.GetTypeDescriptor(type);
        }
    }
}

