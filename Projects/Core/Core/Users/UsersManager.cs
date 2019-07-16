using OnUtils.Application;
using OnUtils.Application.DB;
using OnUtils.Application.Journaling;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace OnWeb.Core.Users
{
    /// <summary>
    /// Представляет менеджер, позволяющий управлять данными пользователей.
    /// </summary>
    public sealed class UsersManager : CoreComponentBase, IComponentSingleton, IUnitOfWorkAccessor<DB.CoreContext>
    {
        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion

        /// <summary>
        /// Возвращает список пользователей, у которых есть роли из переданного списка
        /// </summary>
        /// <param name="roleIdList">Список ролей для поиска пользователей</param>
        /// <param name="onlyActive">Если true, то возвращает только активных пользователей.</param>
        /// <param name="exceptSuperuser">Если false, то в список будут включены суперпользователи (у суперпользователей по-умолчанию есть все роли).</param>
        /// <param name="orderBy">Сортировка выдачи</param>
        /// <returns>Возвращает список пар {пользователь:список ролей из <paramref name="roleIdList"/>} для пользователей, обладающих ролями из списка.</returns>
        [ApiReversible]
        public Dictionary<DB.User, int[]> UsersByRoles(int[] roleIdList, bool onlyActive = true, bool exceptSuperuser = false, Dictionary<string, bool> orderBy = null)
        {
            try
            {
                if (roleIdList == null || roleIdList.Length == 0) return new Dictionary<DB.User, int[]>();

                if (orderBy != null) throw new ArgumentException("Параметр не поддерживается.", nameof(orderBy));

                using (var db = this.CreateUnitOfWork())
                {
                    var queryBase = db.Users.AsQueryable();

                    if (onlyActive) queryBase = queryBase.Where(x => x.State == 0);

                    var idRoleUser = AppCore.AppConfig.RoleUser;
                    if (!roleIdList.Contains(idRoleUser))
                    {
                        var queryRolesWithUsers = exceptSuperuser ? (from user in queryBase
                                                                     join role in db.RoleUser on user.IdUser equals role.IdUser
                                                                     where roleIdList.Contains(role.IdRole)
                                                                     select new { role.IdUser, role.IdRole }) : (from user in queryBase
                                                                                                                 join role in db.RoleUser on user.IdUser equals role.IdUser
                                                                                                                 where roleIdList.Contains(role.IdRole) || user.Superuser != 0
                                                                                                                 select new { role.IdUser, role.IdRole });

                        var data = queryRolesWithUsers.ToList().GroupBy(x => x.IdUser, x => x.IdRole).ToDictionary(x => x.Key, x => x.Distinct().ToArray());

                        var queryUsers = from user in db.Users
                                         where data.Keys.Contains(user.IdUser)
                                         select user;

                        var data2 = queryUsers.ToList().ToDictionary(x => x, x => data[x.IdUser]);
                        return data2;
                    }
                    else
                    {
                        return queryBase.ToDictionary(x => x, x => new int[] { idRoleUser });
                    }
                }
            }
            catch (ArgumentException) { throw; }
            catch (Exception ex)
            {
                this.RegisterEvent(
                    EventType.Error,
                    "Ошибка получения списка пользователей, обладающих ролями.",
                    $"Идентификаторы ролей: {string.Join(", ", roleIdList)}.\r\nПо активности: {(onlyActive ? "только активных" : "всех")}.\r\nСуперпользователи: {(exceptSuperuser ? "только если роль назначена напрямую" : "добавлять всегда")}.\r\nСортировка: {orderBy?.ToString()}.",
                    ex);

                CheckBlockingException(ex);
                throw;
            }
        }

        /// <summary>
        /// Возвращает списки ролей указанных пользователей.
        /// </summary>
        /// <param name="userIdList">Список ролей для поиска пользователей.</param>
        /// <returns></returns>
        [ApiReversible]
        public Dictionary<int, List<Role>> RolesByUser(int[] userIdList)
        {
            try
            {
                if (userIdList == null || userIdList.Length == 0) return new Dictionary<int, List<Role>>();

                using (var db = this.CreateUnitOfWork())
                {
                    var query = from roleJoin in db.RoleUser
                                join role in db.Role on roleJoin.IdRole equals role.IdRole
                                where userIdList.Contains(roleJoin.IdUser)
                                group role by roleJoin.IdUser into gr
                                select new { IdUser = gr.Key, Roles = gr.ToList() };

                    return query.ToDictionary(x => x.IdUser, x => x.Roles);
                }
            }
            catch (Exception ex)
            {
                Debug.Logs($"rolesByUser: {userIdList}; {ex.Message}");
                this.RegisterEvent(EventType.Error, "Ошибка получения списка ролей, назначенных пользователям.", $"Идентификаторы пользователей: {(userIdList?.Any() == true ? "не задано" : string.Join(", ", userIdList))}", ex);

                CheckBlockingException(ex);
                throw;
            }
        }

        /// <summary>
        /// Устанавливает новый список пользователей <paramref name="userIdList"/>, обладающих указанной ролью <paramref name="idRole"/>. С пользователей, не включенных в <paramref name="userIdList"/>, либо, если <paramref name="userIdList"/> пуст или равен null, то со всех пользователей, данная роль снимается.
        /// </summary>
        [ApiReversible]
        public NotFound SetRoleUsers(int idRole, IEnumerable<int> userIdList)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Required))
                {
                    if (db.Role.Where(x => x.IdRole == idRole).Count() == 0) return NotFound.NotFound;

                    if (userIdList?.Any() == true)
                    {
                        db.RoleUser.Where(x => x.IdRole == idRole && !userIdList.Contains(x.IdUser)).Delete();

                        var context = AppCore.GetUserContextManager().GetCurrentUserContext();
                        var IdUserChange = context.IdUser;

                        var usersInRole = db.RoleUser.Where(x => x.IdRole == idRole).Select(x => x.IdUser).ToList();
                        userIdList.Where(x => !usersInRole.Contains(x)).ToList().ForEach(IdUser =>
                        {
                            db.RoleUser.Add(new RoleUser()
                            {
                                IdRole = idRole,
                                IdUser = IdUser,
                                IdUserChange = IdUserChange,
                                DateChange = DateTime.Now.Timestamp()
                            });
                        });
                    }
                    else
                    {
                        db.RoleUser.Where(x => x.IdRole == idRole).Delete();
                    }

                    db.SaveChanges();
                    scope.Commit();
                }

                return NotFound.Success;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка при замене пользователей роли.", $"Идентификатор роли: {idRole}\r\nИдентификаторы пользователей: {(userIdList?.Any() == true ? "не задано" : string.Join(", ", userIdList))}", ex);
                CheckBlockingException(ex);
                return NotFound.Error;
            }
        }

        /// <summary>
        /// Удаляет роль <paramref name="idRole"/> у пользователей из списка <paramref name="userIdList"/>. Если <paramref name="userIdList"/> пуст или равен null, то роль снимается со всех пользователей.
        /// </summary>
        [ApiReversible]
        public NotFound RemoveRoleUsers(int idRole, IEnumerable<int> userIdList)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Required))
                {
                    if (db.Role.Where(x => x.IdRole == idRole).Count() == 0) return NotFound.NotFound;

                    if (userIdList?.Any() == true)
                    {
                        db.RoleUser.Where(x => x.IdRole == idRole && userIdList.Contains(x.IdUser)).Delete();
                    }
                    else
                    {
                        db.RoleUser.Where(x => x.IdRole == idRole).Delete();
                    }

                    db.SaveChanges();
                    scope.Commit();
                }

                return NotFound.Success;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка при удалении роли у пользователей.", $"Идентификатор роли: {idRole}\r\nИдентификаторы пользователей: {(userIdList?.Any() == true ? "не задано" : string.Join(", ", userIdList))}", ex);
                CheckBlockingException(ex);
                return NotFound.Error;
            }
        }

        /// <summary>
        /// Добавляет роль <paramref name="idRole"/> пользователям из списка <paramref name="userIdList"/>.
        /// </summary>
        [ApiReversible]
        public NotFound AddRoleUsers(int idRole, IEnumerable<int> userIdList)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Required))
                {
                    if (db.Role.Where(x => x.IdRole == idRole).Count() == 0) return NotFound.NotFound;

                    var context = AppCore.GetUserContextManager().GetCurrentUserContext();
                    var IdUserChange = context.IdUser;

                    db.Users.Where(x => userIdList.Contains(x.IdUser)).ToList().ForEach((DB.User x) =>
                    {
                        db.RoleUser.AddOrUpdate(new RoleUser()
                        {
                            IdRole = idRole,
                            IdUser = x.IdUser,
                            IdUserChange = IdUserChange,
                            DateChange = DateTime.Now.Timestamp()
                        });
                    });

                    db.SaveChanges();
                    scope.Commit();
                }

                return NotFound.Success;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка при регистрации роли для списка пользователей.", $"Идентификатор роли: {idRole}\r\nИдентификаторы пользователей: {(userIdList?.Any() == true ? "не задано" : string.Join(", ", userIdList))}", ex);
                CheckBlockingException(ex);
                return NotFound.Error;
            }
        }

#pragma warning disable CS1591 // todo внести комментарии.
        [ApiReversible]
        public bool getUsers(Dictionary<int, DB.User> users)
        {
            try
            {
                if (users == null || !users.Any()) return true;

                var listIDForRequest = new List<int>();

                foreach (var pair in users)
                    if (pair.Key > 0 && pair.Value == null)
                        if (!listIDForRequest.Contains(pair.Key))
                            listIDForRequest.Add(pair.Key);

                if (listIDForRequest.Count > 0)
                {
                    using (var db = this.CreateUnitOfWork())
                    {
                        var sql = (from p in db.Users where listIDForRequest.Contains(p.IdUser) select p);
                        foreach (var res in sql) users[res.IdUser] = res;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("user: {0}; ", ex.Message);
                this.RegisterEvent(EventType.Error, "Ошибка при получении данных пользователей.", $"Идентификаторы пользователей: {(users?.Any() == true ? "не задано" : string.Join(", ", users.Keys))}", ex);

                CheckBlockingException(ex);
                throw ex;
            }
        }

        [ApiReversible]
        public IList<DB.UserLogHistory> GetLogHistoryEvents(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                // todo setError(null);

                var dF = dateFrom.Timestamp();
                var dT = dateTo.Timestamp();

                using (var db = this.CreateUnitOfWork())
                {
                    var list = db.UserLogHistory
                                    .Where(x => x.DateEvent >= dF && x.DateEvent <= dT)
                                    .OrderByDescending(x => x.DateEvent).ToList();

                    return list;
                }
            }
            catch (Exception ex)
            {
                CheckBlockingException(ex);
                // todo setError(ex.Message);
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        private void CheckBlockingException(Exception ex)
        {
            var sqlException = ex as SqlException ?? ex.InnerException as SqlException;
            if (sqlException != null)
            {
                if (sqlException.Number == -2)
                {
                    throw new InvalidOperationException("Возможно, целевые таблицы заблокированы вышестоящей транзакцией.");
                }
            }
        }
    }
}
