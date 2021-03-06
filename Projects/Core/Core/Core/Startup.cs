﻿using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Core
{
    class Startup : IConfigureBindings
    {
        void IConfigureBindings<WebApplication>.ConfigureBindings(IBindingsCollection<WebApplication> bindingsCollection)
        {
            bindingsCollection.SetSingleton<Journaling.JournalingManager>();
            bindingsCollection.SetSingleton<ServiceMonitor.Monitor>();
            bindingsCollection.SetSingleton<Users.IEntitiesManager, Users.EntitiesManager>();
            bindingsCollection.SetSingleton<Users.WebUserContextManager>();
            bindingsCollection.SetSingleton<Users.UsersManager>();
        }
    }
}
