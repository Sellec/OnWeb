using System;
using System.Linq;
using System.Reflection;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        OnWeb.Utils.AssemblyResolver.ReflectionAssemblyResolver.RedirectAssembly();
    }
}
