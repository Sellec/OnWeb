using System;
using System.Collections.Generic;

namespace OnWeb.Plugins.Admin
{
    using Core.Modules;
    using Core.Items;

    /// <summary>
    /// Модуль доступа в панель управления.
    /// </summary>
    [ModuleCore("Панель управления", DefaultUrlName = "Admin")]
    public class Module : ModuleCore<Module>
    {
        /// <summary>
        /// Возвращает список меню для всех модулей системы.
        /// </summary>
        public virtual Dictionary<ModuleCore, List<ItemBase>> GetAdminMenuList()
        {
            throw new NotImplementedException();
        }
    }
}
