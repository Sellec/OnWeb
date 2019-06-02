using OnUtils.Architecture.AppCore;
using System;
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
        /// <param name="roleIdList">Список ролей для поиска пользователей</param>
        /// <param name="onlyActive">Если true, то возвращает только активных пользователей.</param>
        /// <param name="exceptSuperuser">Если false, то в список будут включены суперпользователи (у суперпользователей по-умолчанию есть все роли).</param>
        /// <param name="orderBy">Сортировка выдачи</param>
        /// <returns>Возвращает список пар {пользователь:список ролей из <paramref name="roleIdList"/>} для пользователей, обладающих ролями из списка.</returns>
        Dictionary<DB.User, int[]> UsersByRoles(int[] roleIdList, bool onlyActive = true, bool exceptSuperuser = false, Dictionary<string, bool> orderBy = null);

        /// <summary>
        /// Возвращает списки ролей указанных пользователей.
        /// </summary>
        /// <param name="userIdList">Список ролей для поиска пользователей.</param>
        /// <returns></returns>
        Dictionary<int, List<DB.Role>> RolesByUser(int[] userIdList);

        /// <summary>
        /// Устанавливает новый список пользователей <paramref name="userIdList"/>, обладающих указанной ролью <paramref name="idRole"/>. С пользователей, не включенных в <paramref name="userIdList"/>, либо, если <paramref name="userIdList"/> пуст или равен null, то со всех пользователей, данная роль снимается.
        /// </summary>
        NotFound SetRoleUsers(int idRole, IEnumerable<int> userIdList);

        /// <summary>
        /// Добавляет роль <paramref name="idRole"/> пользователям из списка <paramref name="userIdList"/>.
        /// </summary>
        NotFound AddRoleUsers(int idRole, IEnumerable<int> userIdList);

#pragma warning disable CS1591 // todo внести комментарии.
        bool getUsers(Dictionary<int, DB.User> users);
    }
}
