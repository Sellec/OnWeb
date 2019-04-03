using OnUtils;
using OnUtils.Application.Users;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OnWeb.Core.Users
{
    using ExecutionAuthResultContext = ExecutionAuthResult<IUserContext>;
    using ExecutionPermissionsResult = ExecutionResult<PermissionsList>;

    /// <summary>
    /// Менеджер, управляющий контекстами пользователей (см. <see cref="IUserContext"/>).
    /// Каждый поток приложения имеет ассоциированный контекст пользователя, от имени которого могут выполняться запросы и выполняться действия. 
    /// Более подробно см. <see cref="UserContextManager{TApplication}.GetCurrentUserContext"/> / <see cref="UserContextManager{TApplication}.SetCurrentUserContext(IUserContext)"/> / <see cref="UserContextManager{TApplication}.ClearCurrentUserContext"/>.
    /// </summary>
    public sealed class UserContextManager : UserContextManager<ApplicationCore>, IUnitOfWorkAccessor<DB.CoreContext>
    {
        public const string RoleUserName = "RoleUser";
        public const string RoleGuestName = "RoleGuest";

        private static IUserContext SystemUserContext { get; } = new UserManager(new DB.User() { id = int.MaxValue - 1, email = string.Empty, phone = string.Empty, Superuser = 1 }, true);

        #region Методы
        /// <summary>
        /// Возвращает контекст системного пользователя.
        /// </summary>
        public override IUserContext GetSystemUserContext()
        {
            return SystemUserContext;
        }

        /// <summary>
        /// Возвращает контекст гостя.
        /// </summary>
        public override IUserContext CreateGuestUserContext()
        {
            return new UserManager(new DB.User() { id = 0, email = string.Empty, phone = string.Empty, name = "Гость", Superuser = 0 }, false);
        }

        #region Login
        private ExecutionAuthResult CheckLogin(int idUser, string login, string password, UnitOfWork<DB.User> db, out DB.User outData)
        {
            outData = null;

            try
            {
                if (idUser <= 0 && string.IsNullOrEmpty(login)) return new ExecutionAuthResult(eAuthResult.WrongAuthData, "Не указаны реквизиты для авторизации!");

                List<DB.User> query = null;
                bool directAuthorize = false;

                // Если в $user передан id и $password не передан вообще.
                if (idUser > 0)
                {
                    query = db.Repo1.Where(x => x.id == idUser).ToList();
                    directAuthorize = true;
                }

                // Если Email
                if (query == null && login.isEmail())
                {
                    switch (AppCore.Config.userAuthorizeAllowed)
                    {
                        case eUserAuthorizeAllowed.Nothing:
                            return new ExecutionAuthResult(eAuthResult.AuthDisabled, "Авторизация запрещена.");

                        case eUserAuthorizeAllowed.OnlyPhone:
                            return new ExecutionAuthResult(eAuthResult.AuthMethodNotAllowed, "Авторизация возможна только по номеру телефона.");

                        case eUserAuthorizeAllowed.EmailAndPhone:
                            query = (from p in db.Repo1 where string.Compare(p.email, login, true) == 0 select p).ToList();
                            break;

                        case eUserAuthorizeAllowed.OnlyEmail:
                            query = (from p in db.Repo1 where string.Compare(p.email, login, true) == 0 select p).ToList();
                            break;
                    }
                }

                // Если номер телефона
                if (query == null)
                {
                    var phone = PhoneBuilder.ParseString(login);
                    if (phone.IsCorrect)
                    {
                        switch (AppCore.Config.userAuthorizeAllowed)
                        {
                            case eUserAuthorizeAllowed.Nothing:
                                return new ExecutionAuthResult(eAuthResult.AuthDisabled, "Авторизация запрещена.");

                            case eUserAuthorizeAllowed.OnlyEmail:
                                return new ExecutionAuthResult(eAuthResult.AuthMethodNotAllowed, "Авторизация возможна только через электронную почту.");

                            case eUserAuthorizeAllowed.EmailAndPhone:
                                query = (from p in db.Repo1 where string.Compare(p.phone, phone.ParsedPhoneNumber, true) == 0 select p).ToList();
                                break;

                            case eUserAuthorizeAllowed.OnlyPhone:
                                query = (from p in db.Repo1 where string.Compare(p.phone, phone.ParsedPhoneNumber, true) == 0 select p).ToList();
                                break;
                        }
                    }
                }

                if (query == null)
                {
                    return new ExecutionAuthResult(eAuthResult.UnknownError, "Что-то пошло не так во время авторизации.");
                }

                if (query.Count == 1)
                {
                    var res = query.First();

                    if (directAuthorize || res.password == UsersExtensions.hashPassword(password))
                    {
                        outData = res;
                        return new ExecutionAuthResult(eAuthResult.Success);
                    }
                    else
                    {
                        return new ExecutionAuthResult(eAuthResult.WrongPassword, "Неверный пароль.");
                    }
                }
                else if (query.Count > 1)
                {
                    AppCore.Get<Messaging.IMessagingManager>().GetCriticalMessagesReceivers().ForEach(x=>x.SendToAdmin("Одинаковые реквизиты входа!", "Найдено несколько пользователей с логином '" + login + "'"));
                    return new ExecutionAuthResult(eAuthResult.MultipleFound, "Найдено несколько пользователей с логином '" + login + "'. Обратитесь к администратору для решения проблемы.");
                }
                else
                {
                    return new ExecutionAuthResult(eAuthResult.NothingFound, $"Пользователь '{login}' не найден в базе данных.");
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка во время поиска и проверки пользователя", $"IdUser={idUser}, Login='{login}'.", null, ex);
                return new ExecutionAuthResult(eAuthResult.UnknownError, "Неизвестная ошибка во время проверки авторизации.");
            }
        }

        /// <summary>
        /// Возвращает контекст пользователя с идентификатором <paramref name="idUser"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionAuthResultContext"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        public ExecutionAuthResultContext CreateUserContext(int idUser)
        {
            return CreateUserContext(idUser, null, null);
        }

        /// <summary>
        /// Возвращает контекст пользователя с указанными реквизитами <paramref name="login"/>/<paramref name="password"/>. 
        /// </summary>
        /// <param name="login">Логин для авторизации. В качестве логина может выступать Email-адрес или номер телефона (в зависимости от настроек системы).</param>
        /// <param name="password">Пароль для авторизации. Должен передаваться в незашифрованном виде.</param>
        /// <returns>Возвращает объект <see cref="ExecutionAuthResultContext"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        public ExecutionAuthResultContext CreateUserContext(string login, string password)
        {
            return CreateUserContext(0, login, password);
        }

        private ExecutionAuthResult<IUserContext> CreateUserContext(int IdUser = 0, string user = null, string password = null)
        {
            var authorizationAttemptsExceeded = false;
            var id = 0;

            using (var db = new UnitOfWork<DB.User>())
            {
                var returnNewFailedResultWithAuthAttempt = new Func<eAuthResult, string, ExecutionAuthResult<IUserContext>>((authResult, message) =>
                {
                    var idEvent = AppCore.ConfigurationOptionGet("eventLoginError", 0);
                    if (idEvent > 0) RegisterLogHistoryEvent(id, idEvent, message);

                    if (id > 0)
                    {
                        db.DataContext.ExecuteQuery(
                            $"UPDATE users SET AuthorizationAttempts = (AuthorizationAttempts + 1){(authorizationAttemptsExceeded ? ", BlockedUntil=@BlockedUntil, BlockedReason=@BlockedReason" : "")} WHERE id=@IdUser",
                            new
                            {
                                IdUser = id,
                                BlockedUntil = DateTime.Now.Timestamp() + AppCore.ConfigurationOptionGet("AuthorizationAttemptsBlock", 0),
                                BlockedReason = AppCore.ConfigurationOptionGet("AuthorizationAttemptsBlockMessage", ""),
                            }
                        );
                    }

                    return new ExecutionAuthResult<IUserContext>(authResult, message + (authorizationAttemptsExceeded ? " " + AppCore.ConfigurationOptionGet("AuthorizationAttemptsMessage", "") : ""));
                });

                try
                {
                    var checkLoginResult = CheckLogin(IdUser, user, password, db, out DB.User res);
                    if (!checkLoginResult.IsSuccess)
                    {
                        return returnNewFailedResultWithAuthAttempt(checkLoginResult.AuthResult, checkLoginResult.Message);
                    }

                    id = res.id;
                    var attempts = AppCore.ConfigurationOptionGet("AuthorizationAttempts", 0);
                    authorizationAttemptsExceeded = attempts > 0 && (res.AuthorizationAttempts + 1) >= attempts;

                    if (res.BlockedUntil > DateTime.Now.Timestamp())
                    {
                        return returnNewFailedResultWithAuthAttempt(eAuthResult.BlockedUntil, "Учетная запись заблокирована до " + (new DateTime()).FromUnixtime(res.BlockedUntil).ToString("yyyy-mm-dd HH:MM") +
                            (!string.IsNullOrEmpty(res.BlockedReason) ? " по причине: " + res.BlockedReason : "."));
                    }

                    var checkStateResult = this.CheckUserState(res, res.Comment);
                    if (!checkStateResult.IsSuccess)
                    {
                        return returnNewFailedResultWithAuthAttempt(checkStateResult.AuthResult, checkStateResult.Message);
                    }

                    AppCore.Get<IUsersManager>().getUsers(new Dictionary<int, DB.User>() { { id, res } });

                    var permissionsResult = GetPermissions(res.id);
                    if (!permissionsResult.IsSuccess)
                    {
                        return returnNewFailedResultWithAuthAttempt(eAuthResult.UnknownError, permissionsResult.Message);
                    }

                    var userContext = new UserManager(res, true, permissionsResult.Result);
                    userContext.Start(AppCore);

                    // TODO перенести это в регистрацию контекста пользователя в сессии в asp.net.
                    //HttpContext.Current.Session["authorized"] = auth;
                    //HttpContext.Current.Session["id"] = id;
                    //HttpContext.Current.Session["UserId"] = id;
                    //HttpContext.Current.Session["Timestamp"] = DateTime.UtcNow.ToString();

                    //////foreach ($this.mExtensions as $k=>$v)
                    //////{
                    //////    $res = $v.login($user,$password);
                    //////    if (!$res) $this.mFailedExtensions[$k] = $k;
                    //////}

                    var idEvent = AppCore.ConfigurationOptionGet("eventLoginSuccess", 0);
                    if (idEvent > 0) RegisterLogHistoryEvent(id, idEvent);

                    res.AuthorizationAttempts = 0;
                    db.SaveChanges();

                    return new ExecutionAuthResult<IUserContext>(eAuthResult.Success, null, userContext);
                }
                catch (Exception ex)
                {
                    this.RegisterEvent(Journaling.EventType.CriticalError, "Неизвестная ошибка во время авторизации пользователя", $"IdUser={IdUser}, Login='{user}'.", null, ex);
                    return new ExecutionAuthResult<IUserContext>(eAuthResult.UnknownError, "Неизвестная ошибка во время получения контекста пользователя.");
                }
            }
        }

        private ExecutionAuthResult CheckUserState(DB.User data, string comment = null)
        {
            if (data.State == DB.UserState.Active)
            {
                if (data.BlockedUntil > DateTime.Now.Timestamp())
                {
                    return new ExecutionAuthResult(eAuthResult.BlockedUntil, "Учетная запись заблокирована до " + (new DateTime()).FromUnixtime(data.BlockedUntil).ToString("yyyy-mm-dd HH:MM") +
                        (!string.IsNullOrEmpty(data.BlockedReason) ? " по причине: " + data.BlockedReason : "."));
                }

                return new ExecutionAuthResult(eAuthResult.Success);
            }
            else if (data.State == DB.UserState.RegisterNeedConfirmation)
            {
                return new ExecutionAuthResult(eAuthResult.RegisterNeedConfirmation, "Необходимо подтвердить регистрацию путем перехода по ссылке из письма, отправленного на указанный при регистрации Email-адрес.");
            }
            else if (data.State == DB.UserState.RegisterWaitForModerate)
            {
                return new ExecutionAuthResult(eAuthResult.RegisterWaitForModerate, "Заявка на регистрацию еще не проверена администратором.");
            }
            else if (data.State == DB.UserState.RegisterDecline)
            {
                var msg = "Заявка на регистрацию отклонена администратором.";
                return new ExecutionAuthResult(eAuthResult.RegisterDecline, !string.IsNullOrEmpty(comment) ? $"{msg}\r\n\r\nПричина: {comment}" : msg);
            }
            else if (data.State == DB.UserState.Disabled)
            {
                var msg = "Учетная запись отключена.";
                return new ExecutionAuthResult(eAuthResult.Disabled, !string.IsNullOrEmpty(comment) ? $"{msg}\r\n\r\nПричина: {comment}" : msg);
            }
            else
            {
                return new ExecutionAuthResult(eAuthResult.UnknownError, "Ошибка при авторизации");
            }
        }
        #endregion

        ///*
        // * Попытка авторизации через сохраненную сессию.
        // * */
        //public void loginFromSession()
        //{
        //    setError(null);

        //    if (HttpContext.Current == null || HttpContext.Current.Session == null) return;

        //    if (HttpContext.Current.Session["authorized"] is int &&
        //        (int)HttpContext.Current.Session["authorized"] == 1 &&
        //        HttpContext.Current.Session["id"] is int)
        //    {
        //        var id = (int)HttpContext.Current.Session["id"];

        //        //Debug.WriteLineNoLog("UserManager.loginFromSession session says is auth={0}, {1}", id, HttpContext.Current.Session.SessionID);

        //        using (var db = new UnitOfWork<DB.User>())
        //        {
        //            var res = db.Repo1.Where(r => r.id == id).FirstOrDefault();

        //            if (res != null)
        //            {
        //                if (this.checkUserState(res, res.Comment) == eAuthResult.Success)
        //                {
        //                    this.isAuthorized = true;
        //                    Users.getUsers(new Dictionary<int, DB.User>() { { id, res } });

        //                    this.isSuperuser = res.Superuser != 0;

        //                    mData = res;

        //                    mPermissions = Users.loadPermissions(res.id);
        //                    checkLogonAs();

        //                    if (HttpContext.Current.Session["lastEnter"] == null)
        //                        HttpContext.Current.Session["lastEnter"] = DateTime.Now.Timestamp();

        //                    var lastEnter = (int)HttpContext.Current.Session["lastEnter"];
        //                    var diff = DateTime.Now.Timestamp() - lastEnter;

        //                    if (diff > 3600 * 4)
        //                    {
        //                        try
        //                        {
        //                            var IdEvent = AppCore.ConfigurationOptionGet("eventLoginUpdate", 0);
        //                            if (IdEvent > 0) UserLogHistoryManager.register(this.getID(), IdEvent, TimeSpan.FromSeconds(diff).ToString(@"d\.hh\:mm\:ss"));
        //                            HttpContext.Current.Session["lastEnter"] = DateTime.Now.Timestamp();
        //                        }
        //                        catch (Exception) { }
        //                    }
        //                }
        //                else this.logout();
        //            }
        //            else this.logout();
        //        }
        //    }
        //    else
        //    {
        //        //Debug.WriteLineNoLog(string.Format("UserManager.loginFromSession session says nothing, {0}", HttpContext.Current.Session.SessionID));
        //    }
        //}

        public void DestroyUserContext(IUserContext context)
        {
            // TODO перенести это в регистрацию контекста пользователя в сессии в asp.net.
            //if (this.isAuthorized)
            //{
            //    //HttpContext.Current.Session["authorized"] = null;
            //    //HttpContext.Current.Session["id"] = null;
            //    //HttpContext.Current.Session["UserId"] = 0;

            //    HttpContext.Current.Session.Abandon();

            //    var IdEvent = AppCore.ConfigurationOptionGet("eventLogout", 0);
            //    if (IdEvent > 0) UserLogHistoryManager.register(this.getID(), IdEvent);

            //    this.isAuthorized = false;
            //    this.mData = null;
            //    this.isSuperuser = false;
            //    this.mPermissions = null;

            //    ////foreach ($this.mExtensions as $k=>$v) $v.logout();
            //}
        }
        #endregion

        private bool RegisterLogHistoryEvent(int IdUser, int IdEventType, string Comment = null)
        {
            try
            {
                // todo setError(null);

                using (var db = this.CreateUnitOfWork())
                {
                    db.UserLogHistory.Add(new DB.UserLogHistory()
                    {
                        IdUser = IdUser,
                        IdEventType = IdEventType,
                        DateEvent = DateTime.Now.Timestamp(),
                        Comment = string.IsNullOrEmpty(Comment) ? "" : Comment,
                        //IP = System.Web.HttpContext.Current.Request.UserHostAddress // todo добавить адрес
                    });

                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                // todo setError(ex.Message);
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        #region Разрешения
        /// <summary>
        /// Возвращает список разрешений для пользователя <paramref name="idUser"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionPermissionsResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        public ExecutionPermissionsResult GetPermissions(int idUser)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var IdRoleUser = AppCore.ConfigurationOptionGet(RoleUserName, 0);
                    var IdRoleGuest = AppCore.ConfigurationOptionGet(RoleGuestName, 0);

                    var perms2 = (from p in db.RolePermission
                                  join ru in db.RoleUser on p.IdRole equals ru.IdRole into gj
                                  from subru in gj.DefaultIfEmpty()
                                  where (subru.IdUser == idUser) || (idUser > 0 && p.IdRole == IdRoleUser) || (idUser == 0 && p.IdRole == IdRoleGuest)
                                  select new { p.IdModule, p.Permission });

                    var perms = new Dictionary<Guid, List<Guid>>();
                    foreach (var res in perms2)
                    {
                        if (!string.IsNullOrEmpty(res.Permission))
                        {
                            var guidModule = GuidIdentifierGenerator.GenerateGuid(GuidType.Module, res.IdModule);
                            var guidPermission = res.Permission.GenerateGuid();

                            if (!perms.ContainsKey(guidModule)) perms.Add(guidModule, new List<Guid>());
                            if (!perms[guidModule].Contains(guidPermission)) perms[guidModule].Add(guidPermission);
                        }
                    }

                    return new ExecutionPermissionsResult(true, null, new PermissionsList(perms));
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка при получении разрешений для пользователя.", $"IdUser={idUser}.", null, ex);
                return new ExecutionPermissionsResult(false, "Ошибка при получении разрешений для пользователя.");
            }
        }
        #endregion
    }
}
