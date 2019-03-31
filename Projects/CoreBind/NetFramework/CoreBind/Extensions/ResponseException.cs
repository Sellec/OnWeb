using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;

public static class ResponseException
{
    public static void SetCookie (this System.Web.HttpResponseBase response, string name, string value, DateTime? expires = null, string path = "/")
    {
        var cookie = new System.Web.HttpCookie(name, value);
        if (expires.HasValue) cookie.Expires = expires.Value;
        cookie.Path = path;
        response.SetCookie(cookie);
    }
}
