using OnUtils.Application.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace OnWeb.Plugins.Customer
{
    using Core.Configuration;
    using Core.DB;
    using Core.Items;
    using Core.Modules;
    using Core.Users;
    using Core.Exceptions;
    using CoreBind.Modules;
    using Core.Modules;
    using Core.Routing;
    using CoreBind.Modules;
    using CoreBind.Routing;
    using Core.DB;
    using Core.Journaling;

    public class ModuleControllerAdmin : ModuleControllerAdmin<ModuleCustomer>
    {
        [ModuleAction("users", ModuleCustomer.PERM_MANAGEUSERS)]
        public virtual ActionResult Users(UserState? state = null)
        {
            var users = state.HasValue ?
                            DB.Users.Where(x => x.State == state.Value).OrderBy(x => x.name).ToList() :
                            DB.Users.OrderBy(x => x.name).ToList();


            this.assign("IsSuperuser", AppCore.GetUserContextManager().GetCurrentUserContext().IsSuperuser);
            return this.display("admin/admin_customer_users.cshtml", users);
        }

        [ModuleAction("users2", ModuleCustomer.PERM_MANAGEUSERS)]
        public virtual ActionResult Users2()
        {
            var users = DB.Users.Where(x => x.State == 0).OrderBy(x => x.name).ToList();

            this.assign("IsSuperuser", AppCore.GetUserContextManager().GetCurrentUserContext().IsSuperuser);
            return this.display("admin/admin_customer_users.cshtml", users);
        }

        [ModuleAction("users_add", ModuleCustomer.PERM_MANAGEUSERS)]
        public ActionResult UserAdd()
        {
            return UserEdit(0);
        }

        [ModuleAction("users_edit", ModuleCustomer.PERM_MANAGEUSERS)]
        public virtual ActionResult UserEdit(int IdUser = 0)
        {
            var data = IdUser != 0 ? DB.Users.Where(x => x.id == IdUser).FirstOrDefault() : new User();
            if (data == null) throw new KeyNotFoundException("Неправильно указан пользователь!");
            var history = AppCore.Get<IJournalingManager>().GetJournalForItem(data);

            var model = new Model.AdminUserEdit()
            {
                history = history.Result,
                User = data,
                UserRoles = DB.RoleUser
                                .Where(x => x.IdUser == data.id)
                                .Select(x => x.IdRole),
                Roles = DB.Role
                            .OrderBy(x => x.NameRole)
                            .Select(x => new SelectListItem()
                            {
                                Value = x.IdRole.ToString(),
                                Text = x.NameRole
                            }),
            };

            return this.display("admin/admin_customer_users_ae.cshtml", model);
        }

        [ModuleAction("usersSave", ModuleCustomer.PERM_MANAGEUSERS)]
        public virtual ActionResult UserSave(int IdUser = 0, Model.AdminUserEdit model = null)
        {
            var result = JsonAnswer<int>();

            try
            {
                if (IdUser < 0) throw new Exception("Не указан пользователь!");
                else
                {
                    int id = 0;

                    User data = null;
                    UserState oldState = 0;

                    if (IdUser > 0)
                    {
                        data = DB.Users.Where(u => u.id == IdUser).FirstOrDefault();
                        if (data == null) ModelState.AddModelError("IdUser", "Неправильно указан пользователь!");
                        else if (data.Superuser != 0 && AppCore.GetUserContextManager().GetCurrentUserContext().IsSuperuser) ModelState.AddModelError("IdUser", "У вас нет прав на редактирование суперпользователей - это могут делать только другие суперпользователи!");
                        else
                        {
                            oldState = data.State;
                            id = IdUser;
                        }
                    }
                    else
                    {
                        data = new User()
                        {
                            salt = "",
                            DateReg = DateTime.Now.Timestamp(),
                            IP_reg = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.ServerVariables["REMOTE_ADDR"]
                        };
                    }

                    var errors = new List<string>();

                    if (ModelState.ContainsKeyCorrect("User.email")) data.email = model.User.email?.ToLower();
                    if (ModelState.ContainsKeyCorrect("User.phone")) data.phone = UsersExtensions.preparePhone(model.User.phone);

                    if (ModelState.ContainsKeyCorrect("User.name")) data.name = model.User.name;
                    if (Request.Form.HasKey("login") && !string.IsNullOrEmpty(Request.Form["login"]))
                    {
                        if (!Request.Form["login"].isOneStringTextOnly()
                            // todo переработать этот участок в нормальную модель || DataManager.check(Request.Form["login"])
                            ) errors.Add("Некорректный ввод поля login!");
                        else data.name = Request.Form["login"];
                    }

                    if (ModelState.ContainsKeyCorrect("User.name")) data.name = model.User.name;
                    if (ModelState.ContainsKeyCorrect("User.about")) data.about = model.User.about;

                    if (ModelState.ContainsKeyCorrect("User.Superuser"))
                    {
                        if (!AppCore.GetUserContextManager().GetCurrentUserContext().IsSuperuser) errors.Add("Недостаточно прав для установки или снятия признака суперпользователя!");
                        data.Superuser = (byte)(model.User.Superuser == 0 ? 0 : 1);
                    }

                    if (ModelState.ContainsKeyCorrect("User.State"))
                    {
                        switch (model.User.State)
                        {
                            case UserState.Active:
                            case UserState.RegisterNeedConfirmation:
                            case UserState.RegisterWaitForModerate:
                            case UserState.RegisterDecline:
                            case UserState.Disabled:
                                data.State = model.User.State;
                                break;

                            default:
                                ModelState.AddModelError("User.State", "Неизвестное состояние пользователя.");
                                break;
                        }
                    }

                    if (ModelState.ContainsKeyCorrect("User.password"))
                    {
                        if (data.id > 0 && Request.Form.HasKey("changepass") && Request.Form["changepass"] == "2")
                            data.password = UsersExtensions.hashPassword(model.User.password);
                        else if (data.id == 0)
                            data.password = UsersExtensions.hashPassword(model.User.password);
                    }

                    if (ModelState.ContainsKeyCorrect("User.Comment")) data.Comment = model.User.Comment;
                    if (ModelState.ContainsKeyCorrect("User.CommentAdmin")) data.CommentAdmin = model.User.CommentAdmin;
                    if (Request.Form.HasKey("adminComment") && !string.IsNullOrEmpty(Request.Form["adminComment"]))
                    {
                        if (!Request.Form["adminComment"].isOneStringTextOnly()
                            // todo переработать этот участок в нормальную модель || DataManager.check(Request.Form["adminComment"])
                            ) errors.Add("Некорректный ввод комментария администратора!");
                        else data.CommentAdmin = Request.Form["adminComment"];
                    }

                    result.Message = errors.Count > 0 ? " - " + string.Join("\r\n - ", errors) : "";

                    if (errors.Count == 0 && ModelState.IsValid)
                    {
                        data.Fields.CopyValuesFrom(model.User.Fields);
                        data.DateChangeBase = DateTime.Now;
                        data.IdUserChange = AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser();

                        using (var trans = new TransactionScope())
                        {
                            if (data.id == 0) DB.Users.Add(data);

                            if (DB.SaveChanges<User>() > 0)
                            {
                                result.Message = "Сохранение данных прошло успешно!";
                                result.Success = true;

                                Module.RegisterEventForItem(data, EventType.Info, "Редактирование данных", $"Пользователь №{data.id} '" + data.ToString() + "'");

                                //                            if (!($res = $this->mExtensions['fields"]->savePostData($IdUser)))
                                //                    {
                                //$result = $res;
                                //$success = false;
                                //                            }

                                //                             if ($success)
                                //{
                                //                                 if (isset(Request.Form["user_changepass"]) && Request.Form["user_changepass"] == 2 && !(strlen(@Request.Form["user_password"]) == 0))
                                //                                 {
                                //		$exts = $this->getUserManager()->getExtensionsWithMethod('userPassChange');
                                //                                     foreach ( $exts as $k =>$v) $v->userPassChange($IdUser, Request.Form["user_password"]);

                                //		$success = true;
                                //                                 }
                                //                             }

                                if (result.Success)
                                {
                                    {
                                        var rolesMustHave = new List<int>(model.UserRoles ?? new List<int>());
                                        DB.RoleUser.Where(x => x.IdUser == data.id).Delete();
                                        rolesMustHave.ForEach(x => DB.RoleUser.Add(new RoleUser()
                                        {
                                            IdRole = x,
                                            IdUser = data.id,
                                            IdUserChange = AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser(),
                                            DateChange = DateTime.Now.Timestamp()
                                        }));

                                        if (rolesMustHave.Count > 0 && DB.SaveChanges<RoleUser>() == 0)
                                            throw new InvalidOperationException("Не удалось задать список ролей для пользователя.");
                                    }


                                    /*
                                     * todo рассылка на мыло и по телефону
                                     * */
                                    if (oldState == UserState.RegisterWaitForModerate && data.State == UserState.Active)
                                    {
                                        this.assign("login", Request.Form["email"]);
                                        this.assign("message", "Ваша заявка была одобрена администратором, вы можете зайти на сайт, используя логин и пароль, указанные при регистрации!");

                                        AppCore.Get<Core.Messaging.Email.IService>().SendMailFromSite(
                                            data.ToString(),
                                            data.email,
                                            "Успешная регистрация на сайте",
                                            this.displayToVar("Register/register_mail2.cshtml")
                                        );

                                        Module.RegisterEventForItem(data, EventType.Info, "Заявка одобрена", "Пользователь №" + data.id + " '" + data.ToString() + "'");
                                    }
                                    if (oldState == UserState.RegisterWaitForModerate && data.State == UserState.RegisterDecline)
                                    {
                                        var message = ".";

                                        //Если администратор указал комментарий к отклонению заявки
                                        if (!string.IsNullOrEmpty(data.CommentAdmin))
                                        {
                                            message = " по следующей причине: " + data.CommentAdmin;
                                            this.assign("comment", data.CommentAdmin);
                                        }

                                        this.assign("login", data.email);
                                        this.assign("message", "Ваша заявка была отклонена администратором" + message);

                                        AppCore.Get<Core.Messaging.Email.IService>().SendMailFromSite(
                                            data.ToString(),
                                            data.email,
                                            "Регистрация на сайте отклонена",
                                            this.displayToVar("Register/register_mail_decline.cshtml")
                                        );

                                        Module.RegisterEventForItem(data, EventType.Info, "Заявка отклонена", "Пользователь №" + data.id + " '" + data.ToString() + "'. Заявка отклонена администратором" + message);
                                    }
                                    if (oldState != data.State && data.State == UserState.Disabled)
                                    {
                                        var message = ".";

                                        //Если администратор указал комментарий к отключению заявки
                                        if (Request.Form.HasKey("adminComment") && !string.IsNullOrEmpty(Request.Form["adminComment"]))
                                        {
                                            message = " по следующей причине: " + Request.Form["adminComment"];
                                            this.assign("comment", Request.Form["adminComment"]);
                                        }
                                        if (Request.Form.HasKey("CommentAdmin") && !string.IsNullOrEmpty(Request.Form["CommentAdmin"]))
                                        {
                                            message = " по следующей причине: " + Request.Form["CommentAdmin"];
                                            this.assign("comment", Request.Form["CommentAdmin"]);
                                        }

                                        this.assign("login", data.email);
                                        this.assign("message", "Ваш аккаунт заблокирован администратором" + message);
                                        AppCore.Get<Core.Messaging.Email.IService>().SendMailFromSite(
                                            data.ToString(),
                                            data.email,
                                            "Аккаунт заблокирован",
                                            this.displayToVar("Register/register_mail_ban.cshtml")
                                        );

                                        Module.RegisterEventForItem(data, EventType.Info, "Аккаунт заблокирован", "Пользователь №" + data.id + " '" + data.ToString() + "'. Аккаунт заблокирован" + message);
                                    }

                                }

                                result.Data = data.id;

                                trans.Complete();
                            }
                            else result.Message = "Сохранение данных провалилось!";
                        }
                    }
                }
            }
            catch (Exception ex) { result.FromException(ex); }

            return this.ReturnJson(result);
        }

        [ModuleAction("users_delete", ModuleCustomer.PERM_MANAGEUSERS)]
        public virtual JsonResult UserDelete(int IdUser = 0)
        {
            var result = JsonAnswer();
            try
            {
                if (IdUser == 0) result.Message = "Не указан пользователь!";
                else if (IdUser == AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser()) result.Message = "Нельзя удалять себя самого!";
                else
                {
                    var data = DB.Users.Where(x => x.id == IdUser).FirstOrDefault();
                    if (data == null) result.Message = "Неправильно указан пользователь!";
                    else
                    {
                        DB.DeleteEntity(data);
                        if (DB.SaveChanges(data.GetType()) > 0)
                        {
                            result.Message = "Удаление прошло успешно!";
                            result.Success = true;

                            //SystemHistoryManager.register($this, "Пользователь №".$data['id"]." '".$data['name"]."'", "Пользователь '".$data['email"]."' удален", "User_$IdUser");
                        }
                        else result.Message = "Не удалось удалить пользователя!";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }

            return ReturnJson(result);
        }

        [ModuleAction("userAs", ModuleCustomer.PERM_MANAGEUSERS)]
        public virtual ActionResult UserShowAs(int IdUser = 0)
        {
            var result = "";
            if (IdUser < 0) result = "Не указан пользователь!";
            else if (!AppCore.GetUserContextManager().GetCurrentUserContext().IsSuperuser) result = "Нельзя делать это без прав суперпользователя!";
            else
            {
                var data = DB.Users.Where(u => u.id == IdUser).FirstOrDefault();
                if (data == null) throw new Exception("Неправильно указан пользователь!");

                Response.SetCookie("LogonSuperuserAs", IdUser.ToString(), DateTime.Now.AddDays(365), "/");
                return new RedirectResult("/");
            }

            throw new Exception(result);
        }

        [ModuleAction("rolesManage", ModuleCustomer.PERM_MANAGEROLES)]
        public virtual ActionResult RolesManage()
        {
            var model = new Model.AdminRolesManage();

            var perms = (from p in DB.RolePermission
                         group p by p.IdRole into gr
                         select new { gr.Key, gr })
                            .ToDictionary(x => x.Key,
                                          x => x.gr.Select(p => string.Format("{0};{1}", p.IdModule, p.Permission)).ToList());


            model.Roles = DB.Role
                                .OrderBy(x => x.NameRole)
                                .ToDictionary(x => x.IdRole,
                                              x => new Model.AdminRoleEdit(x)
                                              {
                                                  Permissions = perms.ContainsKey(x.IdRole) ? perms[x.IdRole] : new List<string>()
                                              });

            var mperms = new List<SelectListItem>();
            foreach (var module in AppCore.GetModulesManager().GetModules().OrderBy(x => x.Caption))
            {
                var gr = new SelectListGroup() { Name = module.Caption };
                if (!(module is Admin.ModuleAdmin))
                    mperms.Add(new SelectListItem()
                    {
                        Group = gr,
                        Value = string.Format("{0};{1}", module.ID, ModulesConstants.PermissionManage),
                        Text = "Администрирование: Управление модулем"
                    });

                mperms.AddRange(module.GetPermissions().OrderBy(x => x.Caption).Select(x => new SelectListItem()
                {
                    Group = gr,
                    Value = string.Format("{0};{1}", module.ID, x.Key),
                    Text = x.Caption
                }));
            }
            model.ModulesPermissions = mperms;

            return this.display("admin/admin_customer_rolesManage.cshtml", model);
        }

        [ModuleAction("roleSave", ModuleCustomer.PERM_MANAGEROLES)]
        public virtual ActionResult RoleSave(Model.AdminRoleEdit model = null)
        {
            var result = JsonAnswer<Model.AdminRoleEdit>();

            try
            {
                if (model == null) throw new Exception("Из формы не были отправлены данные.");

                Role data = null;
                if (model.IdRole > 0)
                {
                    data = DB.Role.Where(x => x.IdRole == model.IdRole).FirstOrDefault();
                    if (data == null) ModelState.AddModelError(nameof(model.IdRole), "Роль с номером {0} не найдена", model.IdRole);
                }
                else
                {
                    data = new Role()
                    {
                        IdUserCreate = AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser(),
                        DateCreate = DateTime.Now.Timestamp()
                    };
                }

                if (ModelState.IsValid)
                {
                    if (ModelState.ContainsKeyCorrect(nameof(model.NameRole)))
                        if (DB.Role.Where(x => x.IdRole != model.IdRole && x.NameRole == model.NameRole).Count() > 0)
                            ModelState.AddModelError(nameof(model.NameRole), "Роль с именем '{0}' уже существует.", model.NameRole);

                    if (ModelState.ContainsKeyCorrect(nameof(model.Permissions)))
                    {
                        if (model.Permissions.Count > 500) ModelState.AddModelError(nameof(model.Permissions), "Количество разрешающих пунктов в роли не может быть больше 500");
                    }
                }

                if (ModelState.IsValid)
                {
                    data.NameRole = model.NameRole;
                    data.IdUserChange = AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser();
                    data.DateChange = DateTime.Now.Timestamp();

                    if (data.IdRole == 0) DB.Role.Add(data);

                    using (var trans = new TransactionScope())
                    {         
                        if (DB.SaveChanges(data.GetType()) > 0)
                        {
                            model.IdRole = data.IdRole;

                            DB.RolePermission.Where(x => x.IdRole == data.IdRole).Delete();

                            if (ModelState.ContainsKeyCorrect(nameof(model.Permissions)))
                            {
                                model.Permissions.ForEach(x =>
                                {
                                    if (x.IndexOf(';') > 0)
                                    {
                                        var d = x.Split(new char[] { ';' }, 2);
                                        if (d.Length == 2)
                                        {
                                            int moduleID = 0;
                                            if (int.TryParse(d[0], out moduleID))
                                            {
                                                DB.RolePermission.Add(new RolePermission()
                                                {
                                                    IdModule = moduleID,
                                                    IdRole = data.IdRole,
                                                    Permission = d[1],
                                                    IdUserChange = AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser(),
                                                    DateChange = DateTime.Now.Timestamp()
                                                });

                                            }
                                        }
                                    }
                                });

                                if (DB.SaveChanges<RolePermission>() == 0)
                                    throw new Exception($"Возникли ошибки при сохранении разрешений для роли '{data.NameRole}'");
                            }

                            Module.RegisterEventForItem(data, EventType.Info, "Роль обновлена", $"Роль №{data.IdRole} '{data.NameRole}'");

                            trans.Complete();

                            result.Message = "Сохранение роли прошло успешно!";
                            result.Success = true;

                            result.Data = new Model.AdminRoleEdit(data) { Permissions = model.Permissions };
                        }
                        else result.Message = "Не удалось сохранить роль.";
                    }
                }
            }
            catch (Exception ex) { result.FromException(ex); }

            return ReturnJson(result);
        }

        [ModuleAction("roleDelete", ModuleCustomer.PERM_MANAGEROLES)]
        public virtual ActionResult RoleDelete(int IdRole = 0)
        {
            var result = JsonAnswer();

            try
            {
                    var data = DB.Role.Where(x => x.IdRole == IdRole).FirstOrDefault();
                    if (data == null) ModelState.AddModelError(nameof(IdRole), "Роль с номером {0} не найдена", IdRole);

                if (ModelState.IsValid)
                {
                    using (var trans = new TransactionScope())
                    {
                        DB.DeleteEntity(data);
                        DB.RolePermission.Where(x => x.IdRole == IdRole).Delete();

                        if (DB.SaveChanges() > 0)
                        {
                            result.Message = "Удаление роли прошло успешно!";
                            result.Success = true;

                            Module.RegisterEventForItem(data, EventType.Info, "Роль удалена", $"Роль №{data.IdRole} '{data.NameRole}'");

                            trans.Complete();
                        }
                        else result.Message = "Не удалось удалить роль.";
                    }
                }
            }
            catch (Exception ex) { result.FromException(ex); }

            return ReturnJson(result);
        }

        [ModuleAction("rolesDelegate", ModuleCustomer.PERM_MANAGEROLES)]
        public virtual ActionResult RolesDelegate()
        {
            var model = new Model.AdminRolesDelegate();
            using (var db = new CoreContext())
            {
                model.Roles = db.Role.OrderBy(x => x.NameRole).ToList();

                var q = (from u in DB.Users
                         join r in DB.RoleUser on u.id equals r.IdUser into r_j
                         from r in r_j.DefaultIfEmpty()
                         group new { u, r } by u.id into gr
                         select new { User = gr.FirstOrDefault().u, Roles = gr.Where(x => x.r != null).Select(x => x.r.IdRole).ToList() }).ToList();

                model.Users = q.Select(x => x.User).OrderBy(x => x.ToString()).ToList();
                model.RolesUser = q.ToDictionary(x => x.User.id, x => x.Roles);

            }
            return display("admin/admin_customer_rolesDelegate.cshtml", model);
        }

        [ModuleAction("rolesDelegateSave", ModuleCustomer.PERM_MANAGEROLES)]
        public virtual ActionResult RolesDelegateSave([Bind(Prefix = "Roles")] Dictionary<int, List<int>> model = null)
        {
            var result = JsonAnswer();

            try
            {
                using (var trans = DB.CreateScope())
                {
                    DB.RoleUser.Delete();

                    if (model != null)
                        foreach (var user in model)
                        {
                            foreach (var role in user.Value) DB.RoleUser.Add(new RoleUser()
                            {
                                IdRole = role,
                                IdUser = user.Key,
                                IdUserChange = AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser(),
                                DateChange = DateTime.Now.Timestamp()
                            });
                        }

                    DB.SaveChanges();

                    trans.Commit();
                    result.Success = true;
                    result.Message = "Права сохранены";
                }
            }
            catch (Exception ex) { result.FromException(ex); }

            return ReturnJson(result);
        }

        [ModuleAction("history")]
        public virtual ActionResult History(DateTime? datestart = null, DateTime? dateend = null)
        {
            var dateStart = (datestart.HasValue ? datestart.Value : DateTime.Now).Date.AddDays(-365);
            var dateEnd = (dateend.HasValue ? dateend.Value : DateTime.Now).Date.AddDays(1);

            var list = SystemHistoryManager.getList(Module, dateStart, dateEnd);

            return this.display("admin/admin_customer_history.cshtml", new Model.History()
            {
                DateStart = dateStart,
                DateEnd = dateEnd,
                Records = list
            });
        }

        [ModuleAction("historyUserLog")]
        public virtual ActionResult HistoryUserLog(DateTime? datestart = null, DateTime? dateend = null)
        {
            var dateStart = (datestart.HasValue ? datestart.Value : DateTime.Now).Date.AddDays(-1);
            var dateEnd = (dateend.HasValue ? dateend.Value : DateTime.Now).Date.AddDays(1);

            var list = UserLogHistoryManager.getListAll(dateStart, dateEnd);

            return this.display("admin/admin_customer_history_userlog.cshtml", new Model.HistoryUserLog()
            {
                DateStart = dateStart,
                DateEnd = dateEnd,
                Records = list
            });
        }
    }
}
