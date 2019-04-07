using OnUtils.Application.Users;
using OnUtils.Data;
using System;
using System.Linq;

namespace OnWeb.Plugins.Auth
{
    using Core.DB;
    using Core.Modules;

    /// <summary>
    /// Модуль авторизации.
    /// </summary>
    [ModuleCore("Авторизация", DefaultUrlName = "Auth")]
    public abstract class ModuleAuth : ModuleCore<ModuleAuth>, IUnitOfWorkAccessor<CoreContext>
    {
        /// <summary>
        /// Указывает, требуется ли на сайте суперпользователь. Если на данный момент активного суперпользователя нет (нет учетки с пометкой суперпользователя или все суперпользователи заблокированы), то 
        /// модуль регистрации пометит ближайшего зарегистрированного пользователя как суперпользователя и сразу сделает активным.
        /// </summary>
        /// <returns></returns>
        public bool IsSuperuserNeeded()
        {
            using (var db = this.CreateUnitOfWork())
            {
                return db.Users.Where(x => x.Superuser != 0 && x.Block == 0 && x.State == UserState.Active).Count() == 0;
            }
        }

        /// <summary>
        /// Указывает, что на сайте нет ни одного пользователя и требуется немедленная регистрация.
        /// </summary>
        /// <returns></returns>
        public bool IsNeededAnyUserToRegister()
        {
            using (var db = this.CreateUnitOfWork())
            {
                return db.Users.Count() == 0;
            }
        }

        /// <summary>
        /// Запоминает адрес <paramref name="requestedAddress"/>, запрошенный пользователем, ассоциированным с текущим активным контекстом (см. <see cref="UserContextManager{TApplication}.GetCurrentUserContext"/>).
        /// </summary>
        public void RememberUserContextRequestedAddressWhenRedirectedToAuthorization(Uri requestedAddress)
        {
            RememberUserContextRequestedAddressWhenRedirectedToAuthorization(AppCore.GetUserContextManager().GetCurrentUserContext(), requestedAddress);
        }

        /// <summary>
        /// Запоминает адрес <paramref name="requestedAddress"/>, запрошенный пользователем, ассоциированным с указанным контекстом <paramref name="userContext"/>.
        /// </summary>
        public virtual void RememberUserContextRequestedAddressWhenRedirectedToAuthorization(IUserContext userContext, Uri requestedAddress)
        {

        }

        /// <summary>
        /// Возвращает адрес, запомненный модулем во время последнего вызова <see cref="RememberUserContextRequestedAddressWhenRedirectedToAuthorization(IUserContext, Uri)"/> для пользователя, ассоциированного с текущим активным контекстом (см. <see cref="UserContextManager{TApplication}.GetCurrentUserContext"/>).
        /// </summary>
        public Uri GetRememberedUserContextRequestedAddressWhenRedirectedToAuthorization()
        {
            return GetRememberedUserContextRequestedAddressWhenRedirectedToAuthorization(AppCore.GetUserContextManager().GetCurrentUserContext());
        }

        /// <summary>
        /// Возвращает адрес, запомненный модулем во время последнего вызова <see cref="RememberUserContextRequestedAddressWhenRedirectedToAuthorization(IUserContext, Uri)"/> для пользователя, ассоциированного с указанным контекстом <paramref name="userContext"/>.
        /// </summary>
        public virtual Uri GetRememberedUserContextRequestedAddressWhenRedirectedToAuthorization(IUserContext userContext)
        {
            return null;
        }

        /// <summary>
        /// Возвращает пользовательский контекст на основании данных в текущем запросе.
        /// </summary>
        /// <returns>Возвращает пользовательский контекст или null.</returns>
        /// <exception cref="InvalidOperationException">Возникает, если метод выполняется не в рамках входящего запроса.</exception>
        /// <seealso cref="TryGetUserCredentialsFromRequest(out int?)"/>
        public IUserContext RestoreUserContextFromRequest()
        {
            if (TryGetUserCredentialsFromRequest(out int? idUser))
            {
                var userContextResult = AppCore.GetUserContextManager().CreateUserContext(idUser.Value);
                return userContextResult.Result;
            }

            return null;
        }

        /// <summary>
        /// Возвращает данные пользователя из запроса, если их возможно определить и они корректны. Возвращает успех только в том случае, если реквизиты пользователя прошли проверку.
        /// </summary>
        /// <param name="idUser">После выхода из метода содержит идентификатор пользователя или null, если идентификатор не определен.</param>
        /// <returns>Возвращает true, если данные пользователя были найдены в запросе и false в противном случае.</returns>
        /// <exception cref="InvalidOperationException">Возникает, если метод выполняется не в рамках входящего запроса.</exception>
        /// <seealso cref="RestoreUserContextFromRequest"/>
        protected abstract bool TryGetUserCredentialsFromRequest(out int? idUser);

        /// <summary>
        /// Привязывает указанный контекст пользователя <paramref name="context"/> к текущему запросу таким образом, что последующий вызов <see cref="RestoreUserContextFromRequest"/> восстановит новый контекст, ассоциированный с тем же пользователем.
        /// </summary>
        /// <exception cref="ArgumentException">Возникает, если <paramref name="context"/> ассоциирован с гостем.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="context"/> равен null.</exception>
        /// <exception cref="InvalidOperationException">Возникает, если метод выполняется не в рамках входящего запроса.</exception>
        /// <seealso cref="RestoreUserContextFromRequest"/>
        public abstract void BindUserContextToRequest(IUserContext context);
    }
}
