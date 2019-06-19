using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Binders
{
    /// <summary>
    /// Представляет стандартную реализацию привязки модели с поддержкой пользовательских полей (<see cref="Core.ModuleExtensions.CustomFields.ExtensionCustomsFields"/>.
    /// </summary>
    class TraceModelBinder : DefaultModelBinder
    {
        protected override PropertyDescriptorCollection GetModelProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var props = base.GetModelProperties(controllerContext, bindingContext);
            var properties1 = new List<PropertyDescriptor>();
            var properties2 = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor prop in props)
            {
                if (typeof(Core.ModuleExtensions.CustomFields.Data.DefaultSchemeWData).IsAssignableFrom(prop.PropertyType))
                {
                    properties2.Add(prop);
                }
                else
                {
                    properties1.Add(prop);
                }
            }

            var propertiesAll = new List<PropertyDescriptor>();
            foreach (var p in properties1) propertiesAll.Add(p);
            foreach (var p in properties2) propertiesAll.Add(p);

            var propsNew = new PropertyDescriptorCollection(propertiesAll.ToArray());
            return propsNew;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(Core.ModuleExtensions.CustomFields.Data.DefaultSchemeWData))
            {
                var model = bindingContext.Model;
                bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, bindingContext.Model.GetType());
            }

            return base.BindModel(controllerContext, bindingContext);
        }

    }
}
