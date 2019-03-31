using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel;

namespace OnWeb.CoreBind.Providers
{
    class TraceModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        //protected override ModelMetadata
        //    CreateMetadata(IEnumerable<Attribute> attributes,
        //                   Type containerType,
        //                   Func<object> modelAccessor,
        //                   Type modelType,
        //                   string propertyName)
        //{
        //    /* If containerType is an interface, get the actual type 
        //     and the attributes of the current property on that 
        //     type. */
        //    if (containerType != null
        //        && containerType.IsInterface)
        //    {
        //        var target = modelAccessor.Target;
        //        var container = target.GetType().GetField("container").GetValue(target);
        //        containerType = container.GetType();

        //        var propertyDescriptor = GetTypeDescriptor(containerType).GetProperties()[propertyName];

        //        attributes = FilterAttributes(
        //                        containerType,
        //                        propertyDescriptor,
        //                        propertyDescriptor.Attributes.Cast<Attribute>());
        //    }

        //    var attribs = attributes as IList<Attribute> ?? attributes.ToList();

        //    /* Check that we dont have any DisplayName issues in 
        //       validation that will cause the site to explode with an 
        //       error about DisplayName_Set and no stacktrace */

        //    var metadata = base.CreateMetadata(attribs,
        //                                    containerType,
        //                                    modelAccessor,
        //                                    modelType,
        //                                    propertyName);

        //    if (metadata != null && attribs.Any(a => a is DisplayNameAttribute) &&
        //        String.IsNullOrEmpty(metadata.GetDisplayName()) &&
        //        !String.IsNullOrEmpty(metadata.PropertyName))
        //    {
        //        var ctype = containerType != null ? containerType.ToString() : "UnknownType";

        //        return null;
        //        //                throw new ValidationAttributeConfigurationException(metadata.PropertyName, ctype);
        //    }

        //    return metadata;
        //}

        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            if (modelType == typeof(Core.ModuleExtensions.CustomFields.Data.DefaultSchemeWData))
            {
                //var modelValue = modelAccessor.Invoke();
                //modelType = modelValue.GetType();
            }

            //Debug.WriteLine("CreateMetadata: {0}, {1}, {2}", containerType?.FullName, modelType?.FullName, propertyName);
            var md = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            return md;
        }

        protected override ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
         //   Debug.WriteLine("GetTypeDescriptor: {0}", type.FullName);
            var td = base.GetTypeDescriptor(type);

            return td;
        }

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
      //      Debug.WriteLine("GetMetadataForProperties: {0}, {1}", container?.GetType()?.FullName, containerType?.FullName);

            var metadata = new List<ModelMetadata>(base.GetMetadataForProperties(container, containerType));

            try
            {
                var metadata2 = metadata.GroupBy(x => x.PropertyName.ToLower(),
                                                 x => x,
                                                 (gr, list) => list.FirstOrDefault()).Select(x => x).ToList();

                return metadata2;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("InterfaceModelViewDataMetadataProvider.GetMetadataForProperties={0}, {1}", containerType.FullName, ex.Message);
            }

            return metadata;
        }
    }
}

