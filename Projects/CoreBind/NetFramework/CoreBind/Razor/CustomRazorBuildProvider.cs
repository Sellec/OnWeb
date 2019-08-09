using System.CodeDom.Compiler;
using System.Web.WebPages.Razor;

namespace OnWeb.CoreBind.Razor
{
    class CustomRazorBuildProvider : RazorBuildProvider
    {
        protected override void OnBeforeCompilePath(CompilingPathEventArgs args)
        {
            base.OnBeforeCompilePath(args);

            if (!(args.Host is CustomMvcWebPageRazorHost)) args.Host = CustomMvcWebPageRazorHost.CreateFromAnother(args.Host);
        }

        public override void ProcessCompileErrors(CompilerResults results)
        {
            base.ProcessCompileErrors(results);
        }

        protected override WebPageRazorHost GetHostFromConfig()
        {
            var host = base.GetHostFromConfig();
            if (!(host is CustomMvcWebPageRazorHost)) host = CustomMvcWebPageRazorHost.CreateFromAnother(host);

            return host;
        }
    }


}
