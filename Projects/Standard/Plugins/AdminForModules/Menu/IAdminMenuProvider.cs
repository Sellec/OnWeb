namespace OnWeb.Plugins.AdminForModules.Menu
{
    using Core.Types;

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