namespace OnWeb.Plugins.AdminForModules.Menu
{
    using Core.Types;

    /// <summary>
    /// Если модуль реализует данный интерфейс, то при получении списка меню для панели управления используется результат вызова <see cref="GetModuleAdminMenuLinks"/>.
    /// </summary>
    public interface IMenuProvider
    {
        /// <summary>
        /// Возвращает список элементов для меню модуля в панели управления.
        /// </summary>
        /// <returns></returns>
        NestedLinkCollection GetModuleAdminMenuLinks();
    }
}