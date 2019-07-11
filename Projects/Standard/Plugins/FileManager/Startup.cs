using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.FileManager
{
    using Core.ModuleExtensions.CustomFields;
    using CustomFieldsFileTypes;
    using Core.Modules;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplicationCore>.ConfigureBindings(IBindingsCollection<WebApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<FileManager>();
            bindingsCollection.SetTransient<IModuleController<FileManager>, FileManagerController>();

            bindingsCollection.SetTransient<ICustomFieldRender<FileImageFieldType>, FileImageFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<FileFieldType>, FileFieldTypeRender>();
        }
    }
}
