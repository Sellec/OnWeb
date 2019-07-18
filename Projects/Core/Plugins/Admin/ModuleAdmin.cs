using OnUtils.Application.Modules;
using OnUtils.Application.Users;
using System;
using System.Collections.Generic;

namespace OnWeb.Plugins.Admin
{
    using Core.Modules;
    using Core.Types;

    /// <summary>
    /// Модуль доступа в панель управления.
    /// </summary>
    [ModuleCore("Панель управления", DefaultUrlName = "Admin")]
    public class ModuleAdmin : ModuleCore<ModuleAdmin>
    {
        /// <summary>
        /// Возвращает список меню для всех модулей системы с проверкой прав для текущего пользователя.
        /// </summary>
        public Dictionary<IModuleCore, NestedLinkCollection> GetAdminMenuList()
        {
            return GetAdminMenuList(AppCore.GetUserContextManager().GetCurrentUserContext());
        }

        /// <summary>
        /// Возвращает список меню для всех модулей системы с проверкой прав для пользователя, ассоциированного с указанным контекстом.
        /// </summary>
        public virtual Dictionary<IModuleCore, NestedLinkCollection> GetAdminMenuList(IUserContext userContext)
        {
            throw new NotImplementedException();
        }

    }
}
