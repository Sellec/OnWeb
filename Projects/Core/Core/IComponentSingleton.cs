using OnUtils.Architecture.AppCore;

namespace OnWeb.Core
{
    /// <summary>
    /// Представляет общий интерфейс компонента ядра, для которого в ядре может существовать только один экземпляр.
    /// </summary>
    public interface IComponentSingleton : IComponentSingleton<WebApplicationBase>
    {
    }
}
