using OnUtils;
using OnUtils.Application.Users;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;

namespace OnWeb.Core.Users
{
    using ExecutionPermissionsResult = ExecutionResult<PermissionsList>;

    /// <summary>
    /// Менеджер, управляющий контекстами пользователей (см. <see cref="IUserContext"/>).
    /// Каждый поток приложения имеет ассоциированный контекст пользователя, от имени которого могут выполняться запросы и выполняться действия. 
    /// Более подробно см. <see cref="UserContextManager{TApplication}.GetCurrentUserContext"/> / <see cref="UserContextManager{TApplication}.SetCurrentUserContext(IUserContext)"/> / <see cref="UserContextManager{TApplication}.ClearCurrentUserContext"/>.
    /// </summary>
    public class UserContextManager : UserContextManager<ApplicationCore>, IUnitOfWorkAccessor<DB.CoreContext>
    {
        public const string RoleUserName = "RoleUser";
        public const string RoleGuestName = "RoleGuest";

        private static IUserContext SystemUserContext { get; } = new UserContext(new DB.User() { id = int.MaxValue - 1, email = string.Empty, phone = string.Empty, Superuser = 1 }, true);

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
            return new UserContext(new DB.User() { id = 0, email = string.Empty, phone = string.Empty, name = "Гость", Superuser = 0 }, false);
        }

        #region Login
        /// <summary>
        /// Возвращает контекст пользователя с идентификатором <paramref name="idUser"/>.
        /// </summary>
        /// <param name="idUser">Идентификатор пользователя.</param>
        /// <param name="userContext">Содержит контекст в случае успеха.</param>
        /// <returns>Возвращает результат создания контекста.</returns>
        [ApiIrreversible]
        public eAuthResult CreateUserContext(int idUser, out IUserContext userContext)
        {
            return CreateUserContext(idUser, null, null, out userContext, out var resultReason);
        }

        /// <summary>
        /// Возвращает контекст пользователя с идентификатором <paramref name="idUser"/>.
        /// </summary>
        /// <param name="idUser">Идентификатор пользователя.</param>
        /// <param name="userContext">Содержит контекст в случае успеха.</param>
        /// <param name="resultReason">Содержит текстовое пояснение к ответу функции.</param>
        /// <returns>Возвращает результат создания контекста.</returns>
        [ApiIrreversible]
        public eAuthResult CreateUserContext(int idUser, out IUserContext userContext, out string resultReason)
        {
            return CreateUserContext(idUser, null, null, out userContext, out resultReason);
        }

        /// <summary>
        /// Возвращает контекст пользователя с указанными реквизитами <paramref name="login"/>/<paramref name="password"/>. 
        /// </summary>
        /// <param name="login">Логин для авторизации. В качестве логина может выступать Email-адрес или номер телефона (в зависимости от настроек системы).</param>
        /// <param name="password">Пароль для авторизации. Должен передаваться в незашифрованном виде.</param>
        /// <param name="userContext">Содержит контекст в случае успеха.</param>
        /// <returns>Возвращает результат создания контекста.</returns>
        [ApiIrreversible]
        public eAuthResult CreateUserContext(string login, string password, out IUserContext userContext)
        {
            return CreateUserContext(0, login, password, out userContext, out var resultReason);
        }

        /// <summary>
        /// Возвращает контекст пользователя с указанными реквизитами <paramref name="login"/>/<paramref name="password"/>. 
        /// </summary>
        /// <param name="login">Логин для авторизации. В качестве логина может выступать Email-адрес или номер телефона (в зависимости от настроек системы).</param>
        /// <param name="password">Пароль для авторизации. Должен передаваться в незашифрованном виде.</param>
        /// <param name="userContext">Содержит контекст в случае успеха.</param>
        /// <param name="resultReason">Содержит текстовое пояснение к ответу функции.</param>
        /// <returns>Возвращает результат создания контекста.</returns>
        [ApiIrreversible]
        public eAuthResult CreateUserContext(string login, string password, out IUserContext userContext, out string resultReason)
        {
            return CreateUserContext(0, login, password, out userContext, out resultReason);
        }

        private eAuthResult CreateUserContext(int IdUser, string user, string password, out IUserContext userContext, out string resultReason)
        {
            var authorizationAttemptsExceeded = false;
            var id = 0;
            userContext = null;
            resultReason = null;

            using (var db = new DB.CoreContext())
            using (var scope = db.CreateScope(TransactionScopeOption.RequiresNew))
            {
                var returnNewFailedResultWithAuthAttempt = new Func<string, string>(message =>
                {
                    var idEvent = AppCore.ConfigurationOptionGet("eventLoginError", 0);
                    if (idEvent > 0) RegisterLogHistoryEvent(db, id, idEvent, message);

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

                    return message + (authorizationAttemptsExceeded ? " " + AppCore.ConfigurationOptionGet("AuthorizationAttemptsMessage", "") : "");
                });

                try
                {
                    var checkLoginResult = CheckLogin(IdUser, user, password, db, out var res);
                    if (!checkLoginResult.IsSuccess)
                    {
                        resultReason = returnNewFailedResultWithAuthAttempt(checkLoginResult.Message);
                        return checkLoginResult.AuthResult;
                    }

                    id = res.id;
                    var attempts = AppCore.ConfigurationOptionGet("AuthorizationAttempts", 0);
                    authorizationAttemptsExceeded = attempts > 0 && (res.AuthorizationAttempts + 1) >= attempts;

                    AppCore.Get<UsersManager>().getUsers(new Dictionary<int, DB.User>() { { id, res } });

                    var context = new UserContext(res, true);
                    context.Start(AppCore);

                    var permissionsResult = TryRestorePermissions(context);
                    if (!permissionsResult.IsSuccess)
                    {
                        resultReason = returnNewFailedResultWithAuthAttempt(permissionsResult.Message);
                        return eAuthResult.UnknownError;
                    }

                    var idEvent = AppCore.ConfigurationOptionGet("eventLoginSuccess", 0);
                    if (idEvent > 0) RegisterLogHistoryEvent(db, id, idEvent);

                    res.AuthorizationAttempts = 0;
                    db.SaveChanges();

                    userContext = context;

                    var checkStateResult = CheckUserState(res, res.Comment);
                    if (checkStateResult.IsSuccess)
                    {
                        return eAuthResult.Success;
                    }
                    else
                    {
                        resultReason = returnNewFailedResultWithAuthAttempt(checkStateResult.Message);
                        return checkStateResult.AuthResult;
                    }
                }
                catch (Exception ex)
                {
                    this.RegisterEvent(Journaling.EventType.CriticalError, "Неизвестная ошибка во время получения контекста пользователя.", $"IdUser={IdUser}, Login='{user}'.", null, ex);
                    userContext = null;
                    resultReason = "Неизвестная ошибка во время получения контекста пользователя.";
                    return eAuthResult.UnknownError;
                }
                finally
                {
                    scope.Commit();
                }
            }
        }

        private ExecutionAuthResult CheckLogin(int idUser, string login, string password, DB.CoreContext db, out DB.User outData)
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
                    query = db.Users.Where(x => x.id == idUser).ToList();
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
                            query = (from p in db.Users where string.Compare(p.email, login, true) == 0 select p).ToList();
                            break;

                        case eUserAuthorizeAllowed.OnlyEmail:
                            query = (from p in db.Users where string.Compare(p.email, login, true) == 0 select p).ToList();
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
                                query = (from p in db.Users where string.Compare(p.phone, phone.ParsedPhoneNumber, true) == 0 select p).ToList();
                                break;

                            case eUserAuthorizeAllowed.OnlyPhone:
                                query = (from p in db.Users where string.Compare(p.phone, phone.ParsedPhoneNumber, true) == 0 select p).ToList();
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
                    AppCore.Get<Messaging.MessagingManager>().GetCriticalMessagesReceivers().ForEach(x => x.SendToAdmin("Одинаковые реквизиты входа!", "Найдено несколько пользователей с логином '" + login + "'"));
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

        public void DestroyUserContext(IUserContext context)
        {

        }
        #endregion

        private bool RegisterLogHistoryEvent(DB.CoreContext db, int IdUser, int IdEventType, string Comment = null)
        {
            try
            {
                // todo setError(null);

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
        [ApiIrreversible]
        public ExecutionPermissionsResult GetPermissions(int idUser)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                {
                    var authCfg = AppCore.Get<Plugins.Auth.ModuleAuth>().GetConfiguration<Plugins.Auth.ModuleConfiguration>();
                    var idRoleUser = authCfg.RoleUser;
                    var idRoleGuest = authCfg.RoleGuest;

                    var perms2 = (from p in db.RolePermission
                                  join ru in db.RoleUser on p.IdRole equals ru.IdRole into gj
                                  from subru in gj.DefaultIfEmpty()
                                  where (subru.IdUser == idUser) || (idUser > 0 && p.IdRole == idRoleUser) || (idUser == 0 && p.IdRole == idRoleGuest)
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

        /// <summary>
        /// Пытается получить текущие разрешения для пользователя, ассоциированного с контекстом <paramref name="context"/>, и задать их контексту.
        /// </summary>
        /// <returns>Возвращает true, если удалось получить разрешения и установить их для переданного контекста.</returns>
        [ApiIrreversible]
        public ExecutionResult TryRestorePermissions(IUserContext context)
        {
            if (context is UserContext userContext)
            {
                var permissionsResult = GetPermissions(userContext.IdUser);
                if (permissionsResult.IsSuccess)
                {
                    userContext.ApplyPermissions(permissionsResult.Result);
                    return new ExecutionResult(true);
                }
                else return new ExecutionResult(false, permissionsResult.Message);
            }
            else return new ExecutionResult(false, "Неподдерживаемый тип контекста.");
        }
        #endregion
    }
}
