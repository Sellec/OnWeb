using System.IO.Compression;
using System.Web.Mvc;

namespace OnWeb.WebUtils
{
    class CompressBehaviourFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isCompressionEnabled = filterContext.ActionDescriptor.GetCustomAttributes(typeof(IgnoreCompressionAttribute), true).Length == 0;

            if (filterContext.HttpContext.Request.Url.ToString().Contains("disableCompress=1"))
            {
                isCompressionEnabled = false;
            }

            if (isCompressionEnabled)
            {
                var encodings = filterContext.HttpContext.Request.Headers.Get("Accept-Encoding");
                if (encodings != null)
                {
                    // Check the browser accepts deflate or gzip (deflate takes preference)
                    encodings = encodings.ToLower();
                    if (encodings.Contains("deflate"))
                    {
                        filterContext.HttpContext.Response.Filter = new DeflateStream(filterContext.HttpContext.Response.Filter, CompressionMode.Compress);
                        filterContext.HttpContext.Response.AppendHeader("Content-Encoding", "deflate");
                        filterContext.HttpContext.Response.AppendHeader("Vary", "Content-Encoding");
                    }
                    else if (encodings.Contains("gzip"))
                    {
                        filterContext.HttpContext.Response.Filter = new GZipStream(filterContext.HttpContext.Response.Filter, CompressionMode.Compress);
                        filterContext.HttpContext.Response.AppendHeader("Content-Encoding", "gzip");
                        filterContext.HttpContext.Response.AppendHeader("Vary", "Content-Encoding");
                    }
                }

            }
        }
    }
}

namespace System.Web.Mvc
{
    /// <summary>
    /// Результат выполнения метода, помеченного данным атрибутом, не сжимается в gzip/deflate.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreCompressionAttribute : Attribute
    {

    }
}
