using OnUtils.Architecture.AppCore;

namespace OnWeb.Core
{
    /// <summary>
    /// Базовая реализация компонента ядра.
    /// </summary>
    public abstract class CoreComponentBase : CoreComponentBase<WebApplication>, IComponentStartable, IComponent
    {
    }
}
