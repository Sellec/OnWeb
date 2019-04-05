using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using OnWeb;

namespace System.Web.Mvc
{
    /// <summary>
    /// </summary>
    public static class HttpContextRoutingExtensions
    {
        const string EXTENSIONPREFIX = "HttpContextExtensions_";

        internal static string RoutingGetURL(this HttpContext context)
        {
            return (string)context.Items[EXTENSIONPREFIX + "URL"];
        }

        internal static string RoutingGetURL(this HttpContextBase context)
        {
            return (string)context.Items[EXTENSIONPREFIX + "URL"];
        }

        public static ApplicationCore GetAppCore(this HttpContextBase context)
        {
            return context.Items.Contains(EXTENSIONPREFIX + "ApplicationCore") ? (ApplicationCore)context.Items[EXTENSIONPREFIX + "ApplicationCore"] : null;
        }

        public static void SetAppCore(this HttpContext context, ApplicationCore appCore)
        {
            context.Items[EXTENSIONPREFIX + "ApplicationCore"] = appCore;
        }
    }
}