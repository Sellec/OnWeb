using OnUtils.Application.Users;
using OnUtils.Architecture.AppCore;
using System;
using System.Web;

namespace OnWeb.CoreBind.Providers
{
    using Core;

    /// <summary>
    /// Управляет процессом определения пользователя во время выполнения запроса и процессом привязки пользователя после авторизации.
    /// </summary>
    public class SessionBinder : CoreComponentBase, IComponentSingleton
    {
        /// <summary>
        /// См. <see cref="CoreComponentBase{TAppCore}.OnStart"/>.
        /// </summary>
        protected override void OnStart()
        {
        }

        /// <summary>
        /// См. <see cref="CoreComponentBase{TAppCore}.OnStop"/>.
        /// </summary>
        protected override void OnStop()
        {
        }

        /// <summary>
        /// Возвращает пользовательский контекст на основании данных в текущем запросе.
        /// </summary>
        /// <returns>Возвращает пользовательский контекст или null.</returns>
        /// <exception cref="InvalidOperationException">Возникает, если метод выполняется не в рамках входящего запроса.</exception>
        /// <seealso cref="BindUserContextToRequest(IUserContext)"/>
        /// <seealso cref="ClearUserContextFromRequest"/>
        /// <seealso cref="TryGetUserCredentialsFromRequest(out int?)"/>
        public IUserContext RestoreUserContextFromRequest()
        {
            if (TryGetUserCredentialsFromRequest(out int? idUser))
            {
                var userContextResult = AppCore.Get<Core.Users.WebUserContextManager>().CreateUserContext(idUser.Value, out var userContext);
                return userContextResult == Core.Users.eAuthResult.Success ? userContext : null;
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
        protected virtual bool TryGetUserCredentialsFromRequest(out int? idUser)
        {
            idUser = null;

            try
            {
                if (HttpContext.Current == null) throw new InvalidOperationException("Метод вызван не в рамках входящего запроса.");
            }
            catch (InvalidOperationException) { throw; }
            catch
            {
                throw new InvalidOperationException("Метод вызван не в рамках входящего запроса.");
            }

            // Session не всегда существует в Application_AcquireRequestState.
            if (HttpContext.Current.Session == null) return false;

            var sessionAuthorized = HttpContext.Current.Session["authorized"];
            var sessionUserId = HttpContext.Current.Session["UserId"];
            var sessionVerificationKey = HttpContext.Current.Session["VerificationKey"];

            if (sessionAuthorized != null && sessionAuthorized.GetType() == typeof(bool))
            {
                var authorized = (bool)sessionAuthorized;
                if (sessionUserId != null && sessionUserId.GetType() == typeof(int))
                {
                    var userId = (int)sessionUserId;
                    if (sessionVerificationKey != null && sessionVerificationKey.GetType() == typeof(string))
                    {
                        var verificationKeySession = (string)sessionVerificationKey;
                        if (authorized == true && userId > 0 && !string.IsNullOrEmpty(verificationKeySession))
                        {
                            var cookiesVerificationKey = HttpContext.Current.Request.Cookies["VerificationKey"];
                            if (cookiesVerificationKey != null && !string.IsNullOrEmpty(cookiesVerificationKey.Value))
                            {
                                var verificationKeyCookies = cookiesVerificationKey.Value;
                                if (verificationKeyCookies == verificationKeySession)
                                {
                                    idUser = userId;
                                    return true;
                                }
                            }
                        }
                    }
                }

            }

            return false;
        }

        /// <summary>
        /// Привязывает указанный контекст пользователя <paramref name="context"/> к текущему запросу таким образом, что последующий вызов <see cref="RestoreUserContextFromRequest"/> восстановит новый контекст, ассоциированный с тем же пользователем.
        /// </summary>
        /// <exception cref="ArgumentException">Возникает, если <paramref name="context"/> ассоциирован с гостем.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="context"/> равен null.</exception>
        /// <exception cref="InvalidOperationException">Возникает, если метод выполняется не в рамках входящего запроса.</exception>
        /// <seealso cref="ClearUserContextFromRequest"/>
        /// <seealso cref="RestoreUserContextFromRequest"/>
        public virtual void BindUserContextToRequest(IUserContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.IsGuest) throw new ArgumentException("Нельзя ассоциировать сессию гостя.", nameof(context));

            try
            {
                if (HttpContext.Current == null) throw new InvalidOperationException("Метод вызван не в рамках входящего запроса.");
            }
            catch (InvalidOperationException) { throw; }
            catch
            {
                throw new InvalidOperationException("Метод вызван не в рамках входящего запроса.");
            }

            var verificationKey = $"{context.IdUser}_{DateTime.Now.Ticks.MD5()}";

            var cookie = HttpContext.Current.Response.Cookies.Get("VerificationKey");
            if (cookie != null) HttpContext.Current.Response.Cookies.Remove("VerificationKey");
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("VerificationKey", verificationKey) { Expires = DateTime.Now.AddYears(1), Path = "/" });

            HttpContext.Current.Session["authorized"] = true;
            HttpContext.Current.Session["UserId"] = context.IdUser;
            HttpContext.Current.Session["Timestamp"] = DateTime.UtcNow.ToString();
            HttpContext.Current.Session["VerificationKey"] = verificationKey;
        }

        /// <summary>
        /// Удаляет текущий контекст пользователя, сбрасывая авторизацию, таким образом, что последующий вызов <see cref="RestoreUserContextFromRequest"/> НЕ восстановит контекст, ассоциированный с тем же пользователем.
        /// </summary>
        /// <seealso cref="BindUserContextToRequest"/>
        /// <seealso cref="RestoreUserContextFromRequest"/>
        public virtual void ClearUserContextFromRequest()
        {
            try
            {
                if (HttpContext.Current == null) throw new InvalidOperationException("Метод вызван не в рамках входящего запроса.");
            }
            catch (InvalidOperationException) { throw; }
            catch
            {
                throw new InvalidOperationException("Метод вызван не в рамках входящего запроса.");
            }

            HttpContext.Current.Response.Cookies.Remove("VerificationKey");

            HttpContext.Current.Session["authorized"] = false;
            HttpContext.Current.Session["UserId"] = 0;
            HttpContext.Current.Session.Remove("Timestamp");
            HttpContext.Current.Session.Remove("VerificationKey");
        }

    }
}
