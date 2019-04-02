using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OnWeb.CoreBind.Routing;

static class HttpContextRoutingExtensions
{
    const string EXTENSIONPREFIX = "HttpContextExtensions_";

    public static string RoutingGetURL(this HttpContext context)
    {
        return (string)context.Items[EXTENSIONPREFIX + "URL"];
    }

    public static string RoutingGetURL(this HttpContextBase context)
    {
        return (string)context.Items[EXTENSIONPREFIX + "URL"];
    }
}

