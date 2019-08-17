using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace OnWeb.WebUtils
{
    class CompressBehaviourFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(IgnoreCompressionAttribute), true).Length == 0)
            {
                PrepareCompression(filterContext.HttpContext.Request, filterContext.HttpContext.Response);
            }
        }

        public static void PrepareCompression(HttpRequestBase request, HttpResponseBase response)
        {
            var isCompressionEnabled = true;

            if (request.Url.ToString().Contains("disableCompress=1"))
            {
                isCompressionEnabled = false;
            }

            if (isCompressionEnabled)
            {
                var encodings = request.Headers.Get("Accept-Encoding");
                if (encodings != null)
                {
                    // Check the browser accepts deflate or gzip (deflate takes preference)
                    encodings = encodings.ToLower();
                    if (encodings.Contains("deflate"))
                    {
                        response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                        response.AppendHeader("Content-Encoding", "deflate");
                        response.AppendHeader("Vary", "Content-Encoding");
                    }
                    else if (encodings.Contains("gzip"))
                    {
                        response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                        response.AppendHeader("Content-Encoding", "gzip");
                        response.AppendHeader("Vary", "Content-Encoding");
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
