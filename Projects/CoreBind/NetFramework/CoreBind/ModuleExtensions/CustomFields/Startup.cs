using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Core.ModuleExtensions.CustomFields
{
    using Core.ModuleExtensions.CustomFields.Field.FieldTypes;

    class Startup : IConfigureBindings<WebApplicationCore>,  IExecuteStart<WebApplicationCore>
    {
        void IConfigureBindings<WebApplicationCore>.ConfigureBindings(IBindingsCollection<WebApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetTransient<ICustomFieldRender<HiddenSimpleMultiLineFieldType>, HiddenSimpleMultiLineFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<HiddenSingleLineFieldType>, HiddenSingleLineFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<SimpleMultiLineFieldType>, SimpleMultiLineFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<SimpleSingleLineFieldType>, SimpleSingleLineFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<SourceMultipleFieldType>, SourceMultipleFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<SourceSingleFieldType>, SourceSingleFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<UnknownFieldType>, UnknownFieldTypeRender>();
        }

        void IExecuteStart<WebApplicationCore>.ExecuteStart(WebApplicationCore core)
        {
            if (!ValueProviderFactories.Factories.Any(x => x is MetadataAndValues.FieldValueProviderFactory))
            {
                ValueProviderFactories.Factories.Add(new MetadataAndValues.FieldValueProviderFactory());
            }
        }
    }
}
