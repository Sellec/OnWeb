using Newtonsoft.Json;
using OnUtils.Application.Users;
using System;
using System.Web;

namespace OnWeb.Plugins.Auth
{
    class Module2 : ModuleAuth
    {
        public override void RememberUserContextRequestedAddressWhenRedirectedToAuthorization(IUserContext userContext, Uri requestedAddress)
        {
            try
            {
                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session["AuthorizationRedirectUrl"] = requestedAddress == null ? null : JsonConvert.SerializeObject(requestedAddress);
                }
            }
            catch
            {
                throw;
            }
        }

        public override Uri GetRememberedUserContextRequestedAddressWhenRedirectedToAuthorization(IUserContext userContext)
        {
            try
            {
                if (HttpContext.Current.Session != null && HttpContext.Current.Session["AuthorizationRedirectUrl"] != null)
                {
                    var serialized = HttpContext.Current.Session["AuthorizationRedirectUrl"] as string;
                    if (!string.IsNullOrEmpty(serialized))
                    {
                        var uri = JsonConvert.DeserializeObject<Uri>(serialized);
                        return uri;
                    }
                }
            }
            catch
            {
                throw;
            }

            return null;
        }

        protected override bool TryGetUserCredentialsFromRequest(out int? idUser)
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

        public override void BindUserContextToRequest(IUserContext context)
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

            var verificationKey = $"{context.GetIdUser()}_{DateTime.Now.Ticks.MD5()}";

            var cookie = HttpContext.Current.Response.Cookies.Get("VerificationKey");
            if (cookie != null) HttpContext.Current.Response.Cookies.Remove("VerificationKey");
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("VerificationKey", verificationKey) { Expires = DateTime.Now.AddYears(1), Path = "/" });

            HttpContext.Current.Session["authorized"] = true;
            HttpContext.Current.Session["UserId"] = context.GetIdUser();
            HttpContext.Current.Session["Timestamp"] = DateTime.UtcNow.ToString();
            HttpContext.Current.Session["VerificationKey"] = verificationKey;
        }

        public override void ClearUserContextFromRequest()
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
