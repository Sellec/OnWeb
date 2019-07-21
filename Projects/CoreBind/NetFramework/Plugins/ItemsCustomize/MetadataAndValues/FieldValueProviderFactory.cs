using System.Web.Mvc;

namespace OnWeb.Plugins.ItemsCustomize.MetadataAndValues
{
    class FieldValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new FieldValueProvider(controllerContext);
        }
    }
}
