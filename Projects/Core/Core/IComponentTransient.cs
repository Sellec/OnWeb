using OnUtils.Architecture.AppCore;

namespace OnWeb.Core
{
    /// <summary>
    /// Представляет общий интерфейс компонента ядра, для которого в ядре может существовать множество экземпляров.
    /// </summary>
    public interface IComponentTransient : IComponentTransient<WebApplicationBase>
    {
    }
}
