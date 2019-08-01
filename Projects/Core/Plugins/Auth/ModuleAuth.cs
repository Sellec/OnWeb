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
        /// Запоминает адрес <paramref name="requestedAddress"/>, запрошенный пользователем, ассоциированным с текущим активным контекстом (см. <see cref="UserContextManager.GetCurrentUserContext"/>).
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
        /// Возвращает адрес, запомненный модулем во время последнего вызова <see cref="RememberUserContextRequestedAddressWhenRedirectedToAuthorization(IUserContext, Uri)"/> для пользователя, ассоциированного с текущим активным контекстом (см. <see cref="UserContextManager.GetCurrentUserContext"/>).
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

    }
}
