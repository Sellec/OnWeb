using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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

        IEnumerable<DB.User> IUsersManager.UsersByRoles(int[] IdRoleList, bool onlyActive, bool exceptSuperuser, Dictionary<string, bool> orderBy)
        {
            try
            {
                if (IdRoleList == null || IdRoleList.Length == 0) return Enumerable.Empty<DB.User>();

                if (orderBy != null) throw new ArgumentException("Параметр не поддерживается.", nameof(orderBy));

                using (var db = this.CreateUnitOfWork())
                {
                    var query = db.Users.AsQueryable();

                    if (onlyActive) query = query.Where(x => x.State == 0);

                    var IdRoleUser = AppCore.ConfigurationOptionGet(UserContextManager.RoleUserName, 0);
                    if (!IdRoleList.Contains(IdRoleUser))
                    {
                        var q = exceptSuperuser ? (from user in query
                                                   join role in db.RoleUser on user.id equals role.IdUser
                                                   where IdRoleList.Contains(role.IdRole)
                                                   group user.id by user.id into id_gr
                                                   select id_gr.Key) : (from user in query
                                                                        join role in db.RoleUser on user.id equals role.IdUser
                                                                        where IdRoleList.Contains(role.IdRole) || user.Superuser != 0
                                                                        group user.id by user.id into id_gr
                                                                        select id_gr.Key);

                        query = from user in db.Users
                                join p in q on user.id equals p
                                select user;
                    }

                    return query.ToList();
                    //$order = count($order) > 0 ? "ORDER BY ".implode(", ", $order) : "";
                }
            }
            catch (Exception ex)
            {
                Debug.Logs($"user: {IdRoleList}; {ex.Message}");
                throw;
            }
        }

        IDictionary<int, List<DB.Role>> IUsersManager.RolesByUser(int[] IdUserList)
        {
            try
            {
                if (IdUserList == null || IdUserList.Length == 0) throw new ArgumentNullException(nameof(IdUserList));

                using (var db = this.CreateUnitOfWork())
                {
                    var query = from roleJoin in db.RoleUser
                                join role in db.Role on roleJoin.IdRole equals role.IdRole
                                where IdUserList.Contains(roleJoin.IdUser)
                                group role by roleJoin.IdUser into gr
                                select new { IdUser = gr.Key, Roles = gr.ToList() };

                    return query.ToDictionary(x => x.IdUser, x => x.Roles);
                }
            }
            catch (Exception ex)
            {
                Debug.Logs($"rolesByUser: {IdUserList}; {ex.Message}");
                throw;
            }
        }

        bool IUsersManager.SetRoleUsers(int IdRole, IEnumerable<int> users)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope())
                {
                    if (users == null) db.RoleUser.Where(x => x.IdRole == IdRole).Delete();
                    else db.RoleUser.Where(x => x.IdRole == IdRole && !users.Contains(x.IdUser)).Delete();

                    var context = AppCore.GetUserContextManager().GetCurrentUserContext();
                    var IdUserChange = context.GetIdUser();

                    if (users != null)
                    {
                        var usersInRole = db.RoleUser.Where(x => x.IdRole == IdRole).Select(x => x.IdUser).ToList();
                        users.Where(x => !usersInRole.Contains(x)).ToList().ForEach(IdUser =>
                        {
                            db.RoleUser.Add(new DB.RoleUser()
                            {
                                IdRole = IdRole,
                                IdUser = IdUser,
                                IdUserChange = IdUserChange,
                                DateChange = DateTime.Now.Timestamp()
                            });
                        });
                    }

                    db.SaveChanges();
                    scope.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Logs($"setRoleUsers: {IdRole}, {users}; {ex}");
                //setError(ex.Message);
                return false;
            }
        }

        bool IUsersManager.AddRoleUsers(int IdRole, IEnumerable<int> users)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope())
                {
                    var role = db.Role.Where(x => x.IdRole == IdRole).FirstOrDefault();
                    if (role == null) throw new Exception("Указанная роль не найдена.");

                    var context = AppCore.GetUserContextManager().GetCurrentUserContext();
                    var IdUserChange = context.GetIdUser();

                    db.Users.Where(x => users.Contains(x.id)).ToList().ForEach((DB.User x) =>
                    {
                        db.RoleUser.AddOrUpdate(new DB.RoleUser()
                        {
                            IdRole = IdRole,
                            IdUser = x.id,
                            IdUserChange = IdUserChange,
                            DateChange = DateTime.Now.Timestamp()
                        });
                    });

                    db.SaveChanges();
                    scope.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Logs($"addRoleUsers: {IdRole}, {users}; {ex}");
                //setError(ex.Message);
                return false;
            }
        }

        bool IUsersManager.getUsers(IDictionary<int, DB.User> users)
        {
            try
            {
                var listIDForRequest = new List<int>();

                foreach (var pair in users)
                    if (pair.Key > 0 && pair.Value == null)
                        if (!listIDForRequest.Contains(pair.Key))
                            listIDForRequest.Add(pair.Key);

                if (listIDForRequest.Count > 0)
                {
                    using (var db = this.CreateUnitOfWork())
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
                {
                    var list = db.UserLogHistory
                                    .Where(x => x.DateEvent >= dF && x.DateEvent <= dT)
                                    .OrderByDescending(x => x.DateEvent).ToList();

                    return list;
                }
            }
            catch (Exception ex)
            {
                // todo setError(ex.Message);
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
