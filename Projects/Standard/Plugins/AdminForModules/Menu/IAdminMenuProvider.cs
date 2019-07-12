using OnUtils.Application.Types;

namespace OnWeb.Plugins.AdminForModules.Menu
{
    // todo оценить нужность интерфейса.
    public interface IMenuProvider
    {
        /// <summary>
        /// Возвращает список элементов для меню модуля в панели управления.
        /// </summary>
        /// <returns></returns>
        NestedLinkCollection GetAdminMenuItemsBase();
    }
}