﻿using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.FileManager
{
    using Core.ModuleExtensions.CustomFields;
    using Core.Modules;
    using CustomFieldsFileTypes;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplicationBase>.ConfigureBindings(IBindingsCollection<WebApplicationBase> bindingsCollection)
        {
            bindingsCollection.SetSingleton<FileManager>();
            bindingsCollection.SetTransient<IModuleController<FileManager>, FileManagerController>();

            bindingsCollection.SetTransient<ICustomFieldRender<FileImageFieldType>, FileImageFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<FileFieldType>, FileFieldTypeRender>();
        }
    }
}
