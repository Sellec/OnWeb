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

    public static void RoutingPrepareURL(this HttpContext context, string url)
    {
        RoutingPrepareURLInternal(context.Items, context, url);
    }

    public static void RoutingPrepareURLInternal(IDictionary items, HttpContext context, string url)
    {
        var _url = UriExtensions.MakeRelativeFromUrl(url);

        items[EXTENSIONPREFIX + "URL"] = _url;
        //items[EXTENSIONPREFIX + "IsAdmin"] = _url.StartsWith("/admin/mnadmin") || _url.StartsWith("/admin/madmin");
        items[EXTENSIONPREFIX + "IsTranslated"] = false;
        items[EXTENSIONPREFIX + "ControllerType"] = ControllerTypeFactory.RoutingPrepareURL(context, url);
    }

    public static string RoutingGetURL(this HttpContext context)
    {
        return (string)context.Items[EXTENSIONPREFIX + "URL"];
    }

    public static string RoutingGetURL(this HttpContextBase context)
    {
        return (string)context.Items[EXTENSIONPREFIX + "URL"];
    }

    public static ControllerType RoutingControllerType(this HttpContext context)
    {
        return (ControllerType)context.Items[EXTENSIONPREFIX + "ControllerType"];
    }

    public static ControllerType RoutingControllerType(this HttpContextBase context)
    {
        return (ControllerType)context.Items[EXTENSIONPREFIX + "ControllerType"];
    }

    public static bool RoutingIsTranslated(this HttpContext context)
    {
        return (bool)context.Items[EXTENSIONPREFIX + "IsTranslated"];
    }

    public static bool RoutingIsRouted(this HttpContextBase context)
    {
        return (bool)context.Items[EXTENSIONPREFIX + "IsTranslated"];
    }

}

