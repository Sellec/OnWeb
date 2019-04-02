using OnUtils;
using OnUtils.Application;
using OnUtils.Application.Modules;
using OnUtils.Data;
using OnUtils.Utils;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.ModuleCore
{
    using Core;

    class Startup : IConfigureBindings<ApplicationCore>
    {
        void IConfigureBindings<ApplicationCore>.ConfigureBindings(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.RegisterBindingConstraintHandler(new BindingConstraint());
            bindingsCollection.SetSingleton<Module>();
        }
    }
}
