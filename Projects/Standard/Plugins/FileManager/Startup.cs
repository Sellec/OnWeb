using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.FileManager
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<FileManager>();
        }
    }
}
