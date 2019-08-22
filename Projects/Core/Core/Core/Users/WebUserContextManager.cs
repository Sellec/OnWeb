using OnUtils.Application;
using OnUtils.Application.Journaling;
using OnUtils.Application.Messaging;
using OnUtils.Application.Users;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Transactions;

namespace OnWeb.Core.Users
{
    /// <summary>
    /// Менеджер, управляющий контекстами пользователей (см. <see cref="IUserContext"/>).
    /// Каждый поток приложения имеет ассоциированный контекст пользователя, от имени которого могут выполняться запросы и выполняться действия. 
    /// Более подробно см. <see cref="UserContextManager{TAppCoreSelfReference}.GetCurrentUserContext"/> / <see cref="UserContextManager{TAppCoreSelfReference}.SetCurrentUserContext(IUserContext)"/> / <see cref="UserContextManager{TAppCoreSelfReference}.ClearCurrentUserContext"/>.
    /// </summary>
    public class WebUserContextManager : CoreComponentBase, IComponentSingleton, IUnitOfWorkAccessor<DB.CoreContext>
    {
        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected override void OnStop()
        {
        }
        #endregion

        #region Методы
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
            OnWeb.Modules.Auth.ModuleConfiguration authConfig = null;

            using (var db = new DB.CoreContext())
            using (var scope = db.CreateScope(TransactionScopeOption.RequiresNew))
            {
                var returnNewFailedResultWithAuthAttempt = new Func<string, string>(message =>
                {
                    var idEvent = authConfig.EventLoginError;
                    if (idEvent > 0) RegisterLogHistoryEvent(db, id, idEvent, message);

                    if (id > 0)
                    {
                        db.DataContext.ExecuteQuery(
                            $"UPDATE users SET AuthorizationAttempts = (AuthorizationAttempts + 1){(authorizationAttemptsExceeded ? ", BlockedUntil=@BlockedUntil, BlockedReason=@BlockedReason" : "")} WHERE id=@IdUser",
                            new
                            {
                                IdUser = id,
                                BlockedUntil = DateTime.Now.Timestamp() + authConfig.AuthorizationAttemptsBlock,
                                BlockedReason = authConfig.AuthorizationAttemptsBlockMessage,
                            }
                        );
                    }

                    return message + (authorizationAttemptsExceeded ? " " + authConfig.AuthorizationAttemptsMessage : "");
                });

                try
                {
                    authConfig = AppCore.Get<OnWeb.Modules.Auth.ModuleAuth>()?.GetConfiguration<OnWeb.Modules.Auth.ModuleConfiguration>();

                    var checkLoginResult = CheckLogin(IdUser, user, password, db, out var res);
                    if (!checkLoginResult.IsSuccess)
                    {
                        resultReason = returnNewFailedResultWithAuthAttempt(checkLoginResult.Message);
                        return checkLoginResult.AuthResult;
                    }

                    id = res.IdUser;
                    var attempts = authConfig.AuthorizationAttempts;
                    authorizationAttemptsExceeded = attempts > 0 && (res.AuthorizationAttempts + 1) >= attempts;

                    AppCore.Get<UsersManager>().getUsers(new Dictionary<int, DB.User>() { { id, res } });

                    var context = new UserContext(res, true);
                    ((IComponentStartable)context).Start(AppCore);

                    var permissionsResult = AppCore.GetUserContextManager().GetPermissions(context.IdUser);
                    if (!permissionsResult.IsSuccess)
                    {
                        resultReason = returnNewFailedResultWithAuthAttempt(permissionsResult.Message);
                        return eAuthResult.UnknownError;
                    }
                    context.ApplyPermissions(permissionsResult.Result);

                    var idEvent = authConfig.EventLoginSuccess;
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
                    this.RegisterEvent(EventType.CriticalError, "Неизвестная ошибка во время получения контекста пользователя.", $"IdUser={IdUser}, Login='{user}'.", null, ex);
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
                    query = db.Users.Where(x => x.IdUser == idUser).ToList();
                    directAuthorize = true;
                }

                // Если Email
                if (query == null && login.isEmail())
                {
                    switch (AppCore.WebConfig.userAuthorizeAllowed)
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
                        switch (AppCore.WebConfig.userAuthorizeAllowed)
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
                    else
                    {
                        if (!login.isEmail())
                        {
                            return new ExecutionAuthResult(eAuthResult.WrongAuthData, "Переданные данные не являются ни номером телефона, ни адресом электронной почты.");
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
                    AppCore.Get<MessagingManager<WebApplication>>().GetCriticalMessagesReceivers().ForEach(x => x.SendToAdmin("Одинаковые реквизиты входа!", "Найдено несколько пользователей с логином '" + login + "'"));
                    return new ExecutionAuthResult(eAuthResult.MultipleFound, "Найдено несколько пользователей с логином '" + login + "'. Обратитесь к администратору для решения проблемы.");
                }
                else
                {
                    return new ExecutionAuthResult(eAuthResult.NothingFound, $"Пользователь '{login}' не найден в базе данных.");
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка во время поиска и проверки пользователя", $"IdUser={idUser}, Login='{login}'.", null, ex);
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

    }
}
