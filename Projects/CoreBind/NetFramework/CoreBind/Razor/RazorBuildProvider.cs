using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.Razor.Parser;
using System.Web.WebPages.Razor;

namespace OnWeb.CoreBind.Razor
{
    class RazorBuildProvider : System.Web.WebPages.Razor.RazorBuildProvider
    {
        protected override void OnBeforeCompilePath(CompilingPathEventArgs args)
        {
            base.OnBeforeCompilePath(args);

            if (!(args.Host is RazorHost)) args.Host = RazorHost.CreateFromAnother(args.Host);
        }

        public override void ProcessCompileErrors(CompilerResults results)
        {
            base.ProcessCompileErrors(results);
        }

        protected override System.Web.WebPages.Razor.WebPageRazorHost GetHostFromConfig()
        {
            var host = base.GetHostFromConfig();
            if (!(host is RazorHost)) host = RazorHost.CreateFromAnother(host);

            return host;
        }
    }


}
