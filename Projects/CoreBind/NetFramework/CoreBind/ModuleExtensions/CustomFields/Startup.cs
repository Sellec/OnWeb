using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Core.ModuleExtensions.CustomFields
{
    using Core.ModuleExtensions.CustomFields.Field.FieldTypes;

    class Startup : IConfigureBindings<ApplicationCore>,  IExecuteStart<ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetTransient<ICustomFieldRender<HiddenSimpleMultiLineFieldType>, HiddenSimpleMultiLineFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<HiddenSingleLineFieldType>, HiddenSingleLineFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<SimpleMultiLineFieldType>, SimpleMultiLineFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<SimpleSingleLineFieldType>, SimpleSingleLineFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<SourceMultipleFieldType>, SourceMultipleFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<SourceSingleFieldType>, SourceSingleFieldTypeRender>();
            bindingsCollection.SetTransient<ICustomFieldRender<UnknownFieldType>, UnknownFieldTypeRender>();
        }

        void IExecuteStart<ApplicationCore>.ExecuteStart(ApplicationCore core)
        {
            if (!ValueProviderFactories.Factories.Any(x => x is MetadataAndValues.FieldValueProviderFactory))
            {
                ValueProviderFactories.Factories.Add(new MetadataAndValues.FieldValueProviderFactory());
            }
        }
    }
}
