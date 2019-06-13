using OnUtils.Architecture.AppCore;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Core.ModuleExtensions.CustomFields
{
    class Startup : IExecuteStart<ApplicationCore>
    {
        void IExecuteStart<ApplicationCore>.ExecuteStart(ApplicationCore core)
        {
            //if (!ValueProviderFactories.Factories.Any(x => x is MetadataAndValues.FieldValueProviderFactory))
            //{
            //    ValueProviderFactories.Factories.Add(new MetadataAndValues.FieldValueProviderFactory());
            //}
        }
    }
}
