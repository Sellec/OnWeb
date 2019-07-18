using OnUtils.Application.Modules.Extensions.CustomFields;

namespace OnWeb.Core.Modules.Extensions.CustomFields
{
    public class ExtensionCustomsFieldsBase : ExtensionCustomsFieldsBase<WebApplicationBase>
    {
        public new IModuleCore Module
        {
            get => (IModuleCore)base.Module;
        }
    }
}
