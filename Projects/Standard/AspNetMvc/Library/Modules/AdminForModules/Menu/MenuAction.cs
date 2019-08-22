using System;

namespace OnWeb.Modules.AdminForModules.Menu
{
    using Core.Modules;

    /// <summary>
    /// Указывает, что метод должен быть отражен в меню модуля в панели управления.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MenuActionAttribute : ModuleActionAttribute
    {
        /// <summary>
        /// Создает новый экземпляр атрибута.
        /// </summary>
        /// <param name="caption">Видимое название пункта меню.</param>
        /// <param name="alias">Url-доступное имя пункта меню.</param>
        /// <param name="permission">Разрешение, необходимое для получения доступа к пункту меню.</param>
        public MenuActionAttribute(string caption, string alias = null, string permission = null) : base(alias, permission)
        {
            Caption = caption;
        }
    }
}
