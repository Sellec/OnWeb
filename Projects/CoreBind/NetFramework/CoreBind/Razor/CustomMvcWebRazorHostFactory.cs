using System.Web.Mvc;
using System.Web.WebPages.Razor;

namespace OnWeb.CoreBind.Razor
{
    public class CustomMvcWebRazorHostFactory : MvcWebRazorHostFactory
    {
        public override WebPageRazorHost CreateHost(string virtualPath, string physicalPath)
        {
            return new CustomMvcWebPageRazorHost(virtualPath, physicalPath);
        }
    }
}
