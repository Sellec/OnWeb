using System;
using System.Web.Mvc.Razor;
using System.Web.Razor.Parser;
using System.Web.WebPages.Razor;

namespace OnWeb.CoreBind.Razor
{
    class CustomMvcWebPageRazorHost : MvcWebPageRazorHost
    {
        public static CustomMvcWebPageRazorHost CreateFromAnother(WebPageRazorHost host)
        {
            var newHost = new CustomMvcWebPageRazorHost(host.VirtualPath, host.PhysicalPath); //

            newHost.DefaultBaseClass = typeof(TraceViewPage<>).FullName.Replace("`1", ""); //
            newHost.DefaultClassName = host.DefaultClassName;
            newHost.DefaultDebugCompilation = host.DefaultDebugCompilation;
            newHost.DefaultPageBaseClass = typeof(TraceViewPage<>).FullName.Replace("`1", "");
            newHost.DesignTimeMode = host.DesignTimeMode;
            newHost.EnableInstrumentation = host.EnableInstrumentation;
            newHost.GeneratedClassContext = host.GeneratedClassContext;
            newHost.InstrumentedSourceFilePath = host.InstrumentedSourceFilePath;
            newHost.IsIndentingWithTabs = host.IsIndentingWithTabs;
            newHost.TabSize = host.TabSize;

            foreach (string current in host.NamespaceImports) newHost.NamespaceImports.Add(current);

            return newHost;
        }

        public CustomMvcWebPageRazorHost(string virtualPath, string physicalPath) : base(virtualPath, physicalPath)
        {
            try
            {
                this.NamespaceImports.Add(typeof(OnWeb.NamespaceAnchor).Namespace);
                this.NamespaceImports.Add(typeof(OnWeb.NamespaceAnchor).Namespace + ".Design.Additionals");
                this.NamespaceImports.Add(typeof(OnWeb.NamespaceAnchor).Namespace + ".Design.Additionals.ClassicStructures");
                this.NamespaceImports.Add(typeof(OnUtils.Application.Modules.ModulesConstants).Namespace);

                this.NamespaceImports.Add("System.Web.Helpers");
                this.NamespaceImports.Add("System.Web.Mvc");
                this.NamespaceImports.Add("System.Web.Mvc.Html");
                this.NamespaceImports.Add("System.Web.Routing");
                this.NamespaceImports.Add("System.Collections");

                //this.NamespaceImports.AddRange(Utils.ReflectionHelper.GetEngineNamespaces());

                this.DefaultBaseClass = typeof(TraceViewPage<>).FullName.Replace("`1", "");
                this.DefaultPageBaseClass = typeof(TraceViewPage<>).FullName.Replace("`1", "");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void CheckBaseClass()
        {
            base.DefaultBaseClass = typeof(TraceViewPage<>).FullName.Replace("`1", "");
            base.DefaultPageBaseClass = typeof(TraceViewPage<>).FullName.Replace("`1", "");
            base.DefaultNamespace = typeof(OnWeb.Modules.NamespaceAnchor).Namespace;
        }

        public override ParserBase CreateMarkupParser()
        {
            CheckBaseClass();
            return new HtmlMarkupParser();
        }

        public override string DefaultNamespace
        {
            get
            {
                CheckBaseClass();
                return base.DefaultNamespace;
            }
            set { base.DefaultNamespace = value; }
        }

        public override string DefaultBaseClass
        {
            get
            {
                CheckBaseClass();
                return base.DefaultBaseClass;
            }
            set { base.DefaultBaseClass = value; }
        }

        public override string DefaultClassName
        {
            get
            {
                CheckBaseClass();
                return base.DefaultClassName;
            }
            set { base.DefaultClassName = value; }
        }

    }
}