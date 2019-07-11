using OnUtils.Application.Modules;

namespace OnWeb.Core.Modules
{
    /// <summary>
    /// Представляет контроллер, предоставляющий методы для обработки запросов.
    /// </summary>
    public interface IModuleController<TModule> : IComponentTransient
        where TModule : ModuleCore<TModule>
    {
    }
}
