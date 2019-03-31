using OnWeb.Core;

namespace OnWeb.Plugins.Materials
{
    class Startup : IAssemblyStartup<ApplicationCore>
    {
        void IAssemblyStartup<ApplicationCore>.ConfigureBindings(ApplicationCore.BindingsCollection bindingsCollection)
        {
            bindingsCollection.SetSingleton<IModule, ModuleMaterials>();
        }

        void IAssemblyStartup<ApplicationCore>.ExecuteAfterCoreStart(ApplicationCore core)
        {
        }
    }
}