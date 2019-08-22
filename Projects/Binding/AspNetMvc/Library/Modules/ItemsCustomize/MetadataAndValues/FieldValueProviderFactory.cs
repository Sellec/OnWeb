using System.Web.Mvc;

namespace OnWeb.Modules.ItemsCustomize.MetadataAndValues
{
    class FieldValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            return new FieldValueProvider(controllerContext);
        }
    }
}
