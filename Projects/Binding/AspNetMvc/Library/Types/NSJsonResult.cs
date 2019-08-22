using Newtonsoft.Json;
using System.Web.Mvc;

namespace OnWeb.Types
{
    public class NSJsonResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public override void ExecuteResult(ControllerContext context)
        {
            //if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
            //    string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            //{
            //    throw new InvalidOperationException("GET request not allowed");
            //}

            var response = context.HttpContext.Response;
            response.ContentType = !string.IsNullOrEmpty(this.ContentType) ? this.ContentType : "application/json";

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (this.Data == null)
            {
                return;
            }

            response.Write(JsonConvert.SerializeObject(this.Data, Settings));
        }
    }
}
