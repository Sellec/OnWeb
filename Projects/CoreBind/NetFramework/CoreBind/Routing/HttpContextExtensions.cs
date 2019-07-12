using OnUtils.Application;

namespace System.Web.Mvc
{
    /// <summary>
    /// </summary>
    public static class HttpContextRoutingExtensions
    {
        const string EXTENSIONPREFIX = "HttpContextExtensions_";

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