using OnUtils.Application.Modules.Extensions.CustomFields;

namespace OnWeb.Core.Modules.Extensions.CustomFields
{
    using Types;

    public class ExtensionCustomsFieldsBase : ExtensionCustomsFieldsBase<WebApplicationBase>
    {
        public virtual NestedLinkCollection getAdminMenu()
        {
            return null;
        }

        public new IModuleCore Module
        {
            get => (IModuleCore)base.Module;
        }
    }
}
