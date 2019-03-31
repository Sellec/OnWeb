using OnUtils.Architecture.AppCore;
using System.Collections.Generic;

namespace OnWeb.Core.Users
{
    /// <summary>
    /// Представляет менеджер, позволяющий управлять данными пользователей.
    /// </summary>
    public interface IUsersManager : IComponentSingleton<ApplicationCore>
    {
        /// <summary>
        /// Возвращает список пользователей, у которых есть роли из переданного списка
        /// </summary>
        /// <param name="IdRoleList">Список ролей для поиска пользователей</param>
        /// <param name="onlyActive">Если true, то возвращает только активных пользователей.</param>
        /// <param name="exceptSuperuser">Если true, то суперпользователи не возвращаются (у суперпользователей по-умолчанию есть все роли).</param>
        /// <param name="orderBy">Сортировка выдачи</param>
        /// <returns></returns>
        IEnumerable<DB.User> UsersByRoles(int[] IdRoleList, bool onlyActive = true, bool exceptSuperuser = false, Dictionary<string, bool> orderBy = null);

        /// <summary>
        /// Возвращает списки ролей указанных пользователей.
        /// </summary>
        /// <param name="IdUserList">Список ролей для поиска пользователей</param>
        /// <returns></returns>
        IDictionary<int, List<DB.Role>> RolesByUser(int[] IdUserList);

        /// <summary>
        /// Устанавливает новый список пользователей <paramref name="users"/>, обладающих указанной ролью <paramref name="IdRole"/>.
        /// </summary>
        bool SetRoleUsers(int IdRole, IEnumerable<int> users);

        /// <summary>
        /// Добавляет роль <paramref name="IdRole"/> пользователям из списка <paramref name="users"/>.
        /// </summary>
        bool AddRoleUsers(int IdRole, IEnumerable<int> users);

#pragma warning disable CS1591 // todo внести комментарии.
        bool getUsers(IDictionary<int, DB.User> users);
    }
}
