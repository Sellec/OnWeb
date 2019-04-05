using OnUtils.Architecture.AppCore;

namespace OnWeb.Core.Modules
{
    /// <summary>
    /// Представляет контроллер, предоставляющий методы для обработки запросов.
    /// </summary>
    public interface IModuleController<TModule> : IComponentTransient<ApplicationCore> 
        where TModule : ModuleCore<TModule>
    {
    }
}
