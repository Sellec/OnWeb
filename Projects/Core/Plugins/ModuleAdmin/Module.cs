using System;
using System.Collections.Generic;

namespace OnWeb.Plugins.ModuleAdmin
{
    using Core.Modules;
    using Core.Items;

    /// <summary>
    /// Модуль доступа в панель управления.
    /// </summary>
    [ModuleCore("Панель управления")]
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
