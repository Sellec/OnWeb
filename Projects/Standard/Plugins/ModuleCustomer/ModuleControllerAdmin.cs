﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace OnWeb.Plugins.Customer
{
    using Types;

    public class ModuleControllerAdmin : AdminForModules.ModuleAdminController<Module>
    {
        [ModuleAction("users", Module.PERM_MANAGEUSERS)]
        public virtual ActionResult Users(TraceWeb.DB.UserState? state = null)
        {
            var users = state.HasValue ?
                            DB.Users.Where(x => x.State == state.Value).OrderBy(x => x.name).ToList() :
                            DB.Users.OrderBy(x => x.name).ToList();


            this.assign("IsSuperuser", UserManager.Instance.isSuperuser);
            return this.display("admin/admin_customer_users.cshtml", users);
        }

        [ModuleAction("users2", Module.PERM_MANAGEUSERS)]
        public virtual ActionResult Users2()
        {
            var users = DB.Users.Where(x => x.State == 0).OrderBy(x => x.name).ToList();

            this.assign("IsSuperuser", UserManager.Instance.isSuperuser);
            return this.display("admin/admin_customer_users.cshtml", users);
        }

        [ModuleAction("users_add", Module.PERM_MANAGEUSERS)]
        public ActionResult UserAdd()
        {
            return UserEdit(0);
        }

        [ModuleAction("users_edit", Module.PERM_MANAGEUSERS)]
        public virtual ActionResult UserEdit(int IdUser = 0)
        {
            var data = IdUser != 0 ? DB.Users.Where(x => x.id == IdUser).FirstOrDefault() : new TraceWeb.DB.User();
            if (data == null) throw new KeyNotFoundException("Неправильно указан пользователь!");
            var history = SystemHistoryManager.getListAll(this.Module, $"User_{IdUser}");

            var model = new Model.AdminUserEdit()
            {
                history = history,
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

        [ModuleAction("usersSave", Module.PERM_MANAGEUSERS)]
        public virtual ActionResult UserSave(int IdUser = 0, Model.AdminUserEdit model = null)
        {
            var result = JsonAnswer<int>();

            try
            {
                if (IdUser < 0) throw new Exception("Не указан пользователь!");
                else
                {
                    int id = 0;

                    TraceWeb.DB.User data = null;
                    TraceWeb.DB.UserState oldState = 0;

                    if (IdUser > 0)
                    {
                        data = DB.Users.Where(u => u.id == IdUser).FirstOrDefault();
                        if (data == null) ModelState.AddModelError("IdUser", "Неправильно указан пользователь!");
                        else if (data.Superuser != 0 && UserManager.Instance.isSuperuser) ModelState.AddModelError("IdUser", "У вас нет прав на редактирование суперпользователей - это могут делать только другие суперпользователи!");
                        else
                        {
                            oldState = data.State;
                            id = IdUser;
                        }
                    }
                    else
                    {
                        data = new TraceWeb.DB.User()
                        {
                            salt = "",
                            DateReg = DateTime.Now.Timestamp(),
                            IP_reg = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.ServerVariables["REMOTE_ADDR"]
                        };
                    }

                    var errors = new List<string>();

                    if (ModelState.ContainsKeyCorrect("User.email")) data.email = model.User.email?.ToLower();
                    if (ModelState.ContainsKeyCorrect("User.phone")) data.phone = UserManager.preparePhone(model.User.phone);

                    if (ModelState.ContainsKeyCorrect("User.name")) data.name = model.User.name;
                    if (Request.Form.HasKey("login") && !string.IsNullOrEmpty(Request.Form["login"]))
                    {
                        if (!Request.Form["login"].isOneStringTextOnly() || DataManager.check(Request.Form["login"])) errors.Add("Некорректный ввод поля login!");
                        else data.name = Request.Form["login"];
                    }

                    if (ModelState.ContainsKeyCorrect("User.name")) data.name = model.User.name;
                    if (ModelState.ContainsKeyCorrect("User.about")) data.about = model.User.about;

                    if (ModelState.ContainsKeyCorrect("User.Superuser"))
                    {
                        if (!UserManager.Instance.isSuperuser) errors.Add("Недостаточно прав для установки или снятия признака суперпользователя!");
                        data.Superuser = (byte)(model.User.Superuser == 0 ? 0 : 1);
                    }

                    if (ModelState.ContainsKeyCorrect("User.State"))
                    {
                        switch (model.User.State)
                        {
                            case TraceWeb.DB.UserState.Active:
                            case TraceWeb.DB.UserState.RegisterNeedConfirmation:
                            case TraceWeb.DB.UserState.RegisterWaitForModerate:
                            case TraceWeb.DB.UserState.RegisterDecline:
                            case TraceWeb.DB.UserState.Disabled:
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
                            data.password = UserManager.hashPassword(model.User.password);
                        else if (data.id == 0)
                            data.password = UserManager.hashPassword(model.User.password);
                    }

                    if (ModelState.ContainsKeyCorrect("User.Comment")) data.Comment = model.User.Comment;
                    if (ModelState.ContainsKeyCorrect("User.CommentAdmin")) data.CommentAdmin = model.User.CommentAdmin;
                    if (Request.Form.HasKey("adminComment") && !string.IsNullOrEmpty(Request.Form["adminComment"]))
                    {
                        if (!Request.Form["adminComment"].isOneStringTextOnly() || DataManager.check(Request.Form["adminComment"])) errors.Add("Некорректный ввод комментария администратора!");
                        else data.CommentAdmin = Request.Form["adminComment"];
                    }

                    result.Message = errors.Count > 0 ? " - " + string.Join("\r\n - ", errors) : "";

                    if (errors.Count == 0 && ModelState.IsValid)
                    {
                        data.Fields.CopyValuesFrom(model.User.Fields);
                        data.DateChangeBase = DateTime.Now;
                        data.IdUserChange = UserManager.Instance.getID();

                        using (var trans = new System.Transactions.TransactionScope())
                        {
                            if (data.id == 0) DB.Users.Add(data);

                            if (DB.SaveChanges<TraceWeb.DB.User>() > 0)
                            {
                                result.Message = "Сохранение данных прошло успешно!";
                                result.Success = true;

                                SystemHistoryManager.register(this.Module, $"Пользователь №{data.id} '" + data.ToString() + "'", "Редактирование данных", $"User_{data.id}");

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
                                        rolesMustHave.ForEach(x => DB.RoleUser.Add(new TraceWeb.DB.RoleUser()
                                        {
                                            IdRole = x,
                                            IdUser = data.id,
                                            IdUserChange = UserManager.Instance.getID(),
                                            DateChange = DateTime.Now.Timestamp()
                                        }));

                                        if (rolesMustHave.Count > 0 && DB.SaveChanges<TraceWeb.DB.RoleUser>() == 0)
                                            throw new InvalidOperationException("Не удалось задать список ролей для пользователя.");
                                    }


                                    /*
                                     * todo рассылка на мыло и по телефону
                                     * */
                                    if (oldState == TraceWeb.DB.UserState.RegisterWaitForModerate && data.State == UserState.Active)
                                    {
                                        this.assign("login", Request.Form["email"]);
                                        this.assign("message", "Ваша заявка была одобрена администратором, вы можете зайти на сайт, используя логин и пароль, указанные при регистрации!");

                                        Messaging.Manager.Email.sendMailFromSite(
                                            data.ToString(),
                                            data.email,
                                            "Успешная регистрация на сайте",
                                            this.displayToVar("Register/register_mail2.cshtml")
                                        );

                                        SystemHistoryManager.register(Module, "Пользователь №" + data.id + " '" + data.ToString() + "'", "Заявка одобрена", $"User_{data.id}");
                                    }
                                    if (oldState == TraceWeb.DB.UserState.RegisterWaitForModerate && data.State == TraceWeb.DB.UserState.RegisterDecline)
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

                                        Messaging.Manager.Email.sendMailFromSite(//Отправка письма поставщику с сообщением об отклонении заявки
                                            data.ToString(),
                                            data.email,
                                            "Регистрация на сайте отклонена",
                                            this.displayToVar("Register/register_mail_decline.cshtml")
                                        );

                                        SystemHistoryManager.register(Module, $"Пользователь №{data.id} '" + data.ToString() + "'", "Заявка отклонена администратором" + message, $"User_{data.id}");//Запись в журнал истории
                                    }
                                    if (oldState != data.State && data.State == TraceWeb.DB.UserState.Disabled)
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
                                        Messaging.Manager.Email.sendMailFromSite(//Отправка письма поставщику с сообщением об отключении аккаунта
                                            data.ToString(),
                                            data.email,
                                            "Аккаунт заблокирован",
                                            this.displayToVar("Register/register_mail_ban.cshtml")
                                        );

                                        SystemHistoryManager.register(Module, $"Пользователь №{data.id} '" + data.ToString() + "'", "Аккаунт заблокирован" + message, $"User_{data.id}");//Запись в журнал истории
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

        [ModuleAction("users_delete", Module.PERM_MANAGEUSERS)]
        public virtual JsonResult UserDelete(int IdUser = 0)
        {
            var result = JsonAnswer();
            try
            {
                if (IdUser == 0) result.Message = "Не указан пользователь!";
                else if (IdUser == UserManager.Instance.getID()) result.Message = "Нельзя удалять себя самого!";
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

        [ModuleAction("userAs", Module.PERM_MANAGEUSERS)]
        public virtual ActionResult UserShowAs(int IdUser = 0)
        {
            var result = "";
            if (IdUser < 0) result = "Не указан пользователь!";
            else if (!UserManager.Instance.isSuperuser) result = "Нельзя делать это без прав суперпользователя!";
            else
            {
                var data = DB.Users.Where(u => u.id == IdUser).FirstOrDefault();
                if (data == null) throw new Exception("Неправильно указан пользователь!");

                Response.SetCookie("LogonSuperuserAs", IdUser.ToString(), DateTime.Now.AddDays(365), "/");
                return new RedirectResult("/");
            }

            throw new Exception(result);
        }

        [ModuleAction("rolesManage", Module.PERM_MANAGEROLES)]
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
            foreach (var module in ModulesManager.getModules().OrderBy(x => x.Caption))
            {
                var gr = new SelectListGroup() { Name = module.Caption };
                if (!(module is Modules.Admin.Module))
                    mperms.Add(new SelectListItem()
                    {
                        Group = gr,
                        Value = string.Format("{0};{1}", module.ID,  Modules.ModuleCore.ACCESSADMIN),
                        Text = "Администрирование: Управление модулем"
                    });

                mperms.AddRange(module.getPermissions().OrderBy(x => x.Value).Select(x => new SelectListItem()
                {
                    Group = gr,
                    Value = string.Format("{0};{1}", module.ID, x.Key),
                    Text = x.Value.Caption
                }));
            }
            model.ModulesPermissions = mperms;

            return this.display("admin/admin_customer_rolesManage.cshtml", model);
        }

        [ModuleAction("roleSave", Module.PERM_MANAGEROLES)]
        public virtual ActionResult RoleSave(Model.AdminRoleEdit model = null)
        {
            var result = JsonAnswer<Model.AdminRoleEdit>();

            try
            {
                if (model == null) throw new Exception("Из формы не были отправлены данные.");

                TraceWeb.DB.Role data = null;
                if (model.IdRole > 0)
                {
                    data = DB.Role.Where(x => x.IdRole == model.IdRole).FirstOrDefault();
                    if (data == null) ModelState.AddModelError(nameof(model.IdRole), "Роль с номером {0} не найдена", model.IdRole);
                }
                else
                {
                    data = new TraceWeb.DB.Role()
                    {
                        IdUserCreate = UserManager.Instance.getID(),
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
                    data.IdUserChange = UserManager.Instance.getID();
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
                                                DB.RolePermission.Add(new TraceWeb.DB.RolePermission()
                                                {
                                                    IdModule = moduleID,
                                                    IdRole = data.IdRole,
                                                    Permission = d[1],
                                                    IdUserChange = UserManager.Instance.getID(),
                                                    DateChange = DateTime.Now.Timestamp()
                                                });

                                            }
                                        }
                                    }
                                });

                                if (DB.SaveChanges<TraceWeb.DB.RolePermission>() == 0)
                                    throw new Exception($"Возникли ошибки при сохранении разрешений для роли '{data.NameRole}'");
                            }

                            SystemHistoryManager.register(Module, $"Роль №{data.IdRole} '{data.NameRole}'", "Роль обновлена", $"Role_{data.IdRole}");

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

        [ModuleAction("roleDelete", Module.PERM_MANAGEROLES)]
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

                            SystemHistoryManager.register(Module, $"Роль №{data.IdRole} '{data.NameRole}'", "Роль удалена", $"Role_{data.IdRole}");

                            trans.Complete();
                        }
                        else result.Message = "Не удалось удалить роль.";
                    }
                }
            }
            catch (Exception ex) { result.FromException(ex); }

            return ReturnJson(result);
        }

        [ModuleAction("rolesDelegate", Module.PERM_MANAGEROLES)]
        public virtual ActionResult RolesDelegate()
        {
            var model = new Model.AdminRolesDelegate();
            model.Roles = DB.Role.OrderBy(x => x.NameRole).ToList();

            var q = (from u in DB.Users
                     join r in DB.RoleUser on u.id equals r.IdUser into r_j
                     from r in r_j.DefaultIfEmpty()
                     group new { u, r } by u.id into gr
                     select new { User = gr.FirstOrDefault().u, Roles = gr.Where(x => x.r != null).Select(x => x.r.IdRole).ToList() }).ToList();

            model.Users = q.Select(x => x.User).OrderBy(x => x.ToString()).ToList();
            model.RolesUser = q.ToDictionary(x => x.User.id, x => x.Roles);

            return this.display("admin/admin_customer_rolesDelegate.cshtml", model);
        }

        [ModuleAction("rolesDelegateSave", Module.PERM_MANAGEROLES)]
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
                            foreach (var role in user.Value) DB.RoleUser.Add(new TraceWeb.DB.RoleUser()
                            {
                                IdRole = role,
                                IdUser = user.Key,
                                IdUserChange = UserManager.Instance.getID(),
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
