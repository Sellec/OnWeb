using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.FileManager
{
    using Plugins.ItemsCustomize;
    using Core.Modules;
    using CustomFieldsFileTypes;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<FileManager>();
            bindingsCollection.SetTransient<IModuleController<FileManager>, FileManagerController>();

            bindingsCollection.SetTransient<ICustomFieldRender<FileImageFieldType>, FileImageFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<FileFieldType>, FileFieldTypeRender>();
        }
    }
}
