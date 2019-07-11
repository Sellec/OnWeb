using OnUtils.Application.Items;
using OnUtils.Application.Modules;
using OnUtils.Application.Users;
using System;
using System.Collections.Generic;

namespace OnWeb.Plugins.Admin
{
    /// <summary>
    /// Модуль доступа в панель управления.
    /// </summary>
    [ModuleCore("Панель управления", DefaultUrlName = "Admin")]
    public class ModuleAdmin : ModuleCore<ModuleAdmin>
    {
        /// <summary>
        /// Возвращает список меню для всех модулей системы с проверкой прав для текущего пользователя.
        /// </summary>
        public Dictionary<ModuleCore, List<ItemBase>> GetAdminMenuList()
        {
            return GetAdminMenuList(AppCore.GetUserContextManager().GetCurrentUserContext());
        }

        /// <summary>
        /// Возвращает список меню для всех модулей системы с проверкой прав для пользователя, ассоциированного с указанным контекстом.
        /// </summary>
        public virtual Dictionary<ModuleCore, List<ItemBase>> GetAdminMenuList(IUserContext userContext)
        {
            throw new NotImplementedException();
        }

    }
}
