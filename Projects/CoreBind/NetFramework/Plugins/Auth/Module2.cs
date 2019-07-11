using Newtonsoft.Json;
using OnUtils.Application.Users;
using System;
using System.Web;
using OnUtils.Application.Modules;

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
    }
}
