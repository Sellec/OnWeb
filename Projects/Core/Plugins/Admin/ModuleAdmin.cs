using System;
using System.Collections.Generic;
using OnUtils.Application.Users;

namespace OnWeb.Plugins.Admin
{
    using Core.Modules;
    using Core.Items;

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
