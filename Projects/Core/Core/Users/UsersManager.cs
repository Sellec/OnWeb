using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Data.SqlClient;

namespace OnWeb.Core.Users
{
    //todo привести в порядок.
    class UsersManager : CoreComponentBase<ApplicationCore>, IUsersManager, IUnitOfWorkAccessor<DB.CoreContext>
    {
        #region CoreComponentBase
        protected sealed override void OnStart()
        {
        }

        protected sealed override void OnStop()
        {
        }
        #endregion

        Dictionary<DB.User, int[]> IUsersManager.UsersByRoles(int[] roleIdList, bool onlyActive, bool exceptSuperuser, Dictionary<string, bool> orderBy)
        {
            try
            {
                if (roleIdList == null || roleIdList.Length == 0) return new Dictionary<DB.User, int[]>();

                if (orderBy != null) throw new ArgumentException("Параметр не поддерживается.", nameof(orderBy));

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                {
                    var queryBase = db.Users.AsQueryable();

                    if (onlyActive) queryBase = queryBase.Where(x => x.State == 0);

                    var idRoleUser = AppCore.ConfigurationOptionGet(UserContextManager.RoleUserName, 0);
                    if (!roleIdList.Contains(idRoleUser))
                    {
                        var queryRolesWithUsers = exceptSuperuser ? (from user in queryBase
                                                                     join role in db.RoleUser on user.id equals role.IdUser
                                                                     where roleIdList.Contains(role.IdRole)
                                                                     select new { role.IdUser, role.IdRole }) : (from user in queryBase
                                                                                                                 join role in db.RoleUser on user.id equals role.IdUser
                                                                                                                 where roleIdList.Contains(role.IdRole) || user.Superuser != 0
                                                                                                                 select new { role.IdUser, role.IdRole });

                        var data = queryRolesWithUsers.ToList().GroupBy(x => x.IdUser, x => x.IdRole).ToDictionary(x => x.Key, x => x.Distinct().ToArray());

                        var queryUsers = from user in db.Users
                                         where data.Keys.Contains(user.id)
                                         select user;

                        var data2 = queryUsers.ToList().ToDictionary(x => x, x => data[x.id]);
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
                    Journaling.EventType.Error,
                    "Ошибка получения списка пользователей, обладающих ролями.",
                    $"Идентификаторы ролей: {string.Join(", ", roleIdList)}.\r\nПо активности: {(onlyActive ? "только активных" : "всех")}.\r\nСуперпользователи: {(exceptSuperuser ? "только если роль назначена напрямую" : "добавлять всегда")}.\r\nСортировка: {orderBy?.ToString()}.",
                    ex);

                CheckBlockingException(ex);
                throw;
            }
        }

        Dictionary<int, List<DB.Role>> IUsersManager.RolesByUser(int[] userIdList)
        {
            try
            {
                if (userIdList == null || userIdList.Length == 0) return new Dictionary<int, List<DB.Role>>();

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
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
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка получения списка ролей, назначенных пользователям.", $"Идентификаторы пользователей: {(userIdList?.Any() == true ? "не задано" : string.Join(", ", userIdList))}", ex);

                CheckBlockingException(ex);
                throw;
            }
        }

        NotFound IUsersManager.SetRoleUsers(int idRole, IEnumerable<int> userIdList)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                {
                    if (db.Role.Where(x => x.IdRole == idRole).Count() == 0) return NotFound.NotFound;

                    if (userIdList?.Any() == true)
                    {
                        db.RoleUser.Where(x => x.IdRole == idRole && !userIdList.Contains(x.IdUser)).Delete();

                        var context = AppCore.GetUserContextManager().GetCurrentUserContext();
                        var IdUserChange = context.GetIdUser();

                        var usersInRole = db.RoleUser.Where(x => x.IdRole == idRole).Select(x => x.IdUser).ToList();
                        userIdList.Where(x => !usersInRole.Contains(x)).ToList().ForEach(IdUser =>
                        {
                            db.RoleUser.Add(new DB.RoleUser()
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

                CheckUpdateCurrentUserContext(userIdList);

                return NotFound.Success;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка при замене пользователей роли.", $"Идентификатор роли: {idRole}\r\nИдентификаторы пользователей: {(userIdList?.Any() == true ? "не задано" : string.Join(", ", userIdList))}", ex);
                CheckBlockingException(ex);
                return NotFound.Error;
            }
        }

        NotFound IUsersManager.AddRoleUsers(int idRole, IEnumerable<int> userIdList)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    if (db.Role.Where(x => x.IdRole == idRole).Count() == 0) return NotFound.NotFound;

                    var context = AppCore.GetUserContextManager().GetCurrentUserContext();
                    var IdUserChange = context.GetIdUser();

                    db.Users.Where(x => userIdList.Contains(x.id)).ToList().ForEach((DB.User x) =>
                    {
                        db.RoleUser.AddOrUpdate(new DB.RoleUser()
                        {
                            IdRole = idRole,
                            IdUser = x.id,
                            IdUserChange = IdUserChange,
                            DateChange = DateTime.Now.Timestamp()
                        });
                    });

                    db.SaveChanges();
                    scope.Complete();
                }

                CheckUpdateCurrentUserContext(userIdList);

                return NotFound.Success;
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка при регистрации роли для списка пользователей.", $"Идентификатор роли: {idRole}\r\nИдентификаторы пользователей: {(userIdList?.Any() == true ? "не задано" : string.Join(", ", userIdList))}", ex);
                CheckBlockingException(ex);
                return NotFound.Error;
            }
        }

        private void CheckUpdateCurrentUserContext(IEnumerable<int> userIdList)
        {
            var currentContext = AppCore.GetUserContextManager().GetCurrentUserContext();
            if (userIdList != null && userIdList.Contains(currentContext.GetIdUser()))
            {
                AppCore.GetUserContextManager().TryRestorePermissions(currentContext);
            }
        }

        bool IUsersManager.getUsers(Dictionary<int, DB.User> users)
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
                    using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                    {
                        var sql = (from p in db.Users where listIDForRequest.Contains(p.id) select p);
                        foreach (var res in sql) users[res.id] = res;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("user: {0}; ", ex.Message);
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка при получении данных пользователей.", $"Идентификаторы пользователей: {(users?.Any() == true ? "не задано" : string.Join(", ", users.Keys))}", ex);

                CheckBlockingException(ex);
                throw ex;
            }
        }

        public IList<DB.UserLogHistory> GetLogHistoryEvents(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                // todo setError(null);

                var dF = dateFrom.Timestamp();
                var dT = dateTo.Timestamp();

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
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
