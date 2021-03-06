﻿using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Binding
{
    using Binding.Providers;
    using Modules.Auth;

    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<ModuleAuth, Module2>();
            bindingsCollection.SetSingleton<SessionBinder>();
        }
    }
}
