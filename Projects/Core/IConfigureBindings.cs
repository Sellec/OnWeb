using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb
{
    /// <summary>
    /// При создании и запуске ядра создаются экземпляры всех неабстрактных классов, имеющих открытый беспараметрический конструктор, реализующих данный интерфейс,
    /// после чего для каждого экземпляра вызывается метод <see cref="IConfigureBindings{TAppCore}.ConfigureBindings(IBindingsCollection{TAppCore})"/>.
    /// </summary>
    public interface IConfigureBindings : IConfigureBindings<ApplicationCore>
    {
    }
}
