using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;

namespace OnWeb.CoreBind.Providers
{
    //class my : RazorViewEngine
    //{
    //    public my(IRazorPageFactoryProvider pageFactory, IRazorPageActivator pageActivator, HtmlEncoder htmlEncoder, IOptions<RazorViewEngineOptions> optionsAccessor, RazorProject razorProject, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource) : base(pageFactory, pageActivator, htmlEncoder, optionsAccessor, razorProject, loggerFactory, diagnosticSource)
    //    {
    //    }

    //    public my(IRazorPageFactoryProvider pageFactory, IRazorPageActivator pageActivator, HtmlEncoder htmlEncoder, IOptions<RazorViewEngineOptions> optionsAccessor, RazorProjectFileSystem razorFileSystem, ILoggerFactory loggerFactory, DiagnosticSource diagnosticSource) : base(pageFactory, pageActivator, htmlEncoder, optionsAccessor, razorFileSystem, loggerFactory, diagnosticSource)
    //    {
    //    }

    //    override 
    //}

    class c : IViewLocationExpander
    {
        IEnumerable<string> IViewLocationExpander.ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations;
        }

        void IViewLocationExpander.PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }

    class ResourceProvider : Core.Storage.ResourceProvider//, IViewEngine, IRouteHandler
    {
        private IViewEngine _previousViewEngine = null;

        public ResourceProvider(IViewEngine previousViewEngine)
        {
            _previousViewEngine = previousViewEngine;
        }

        //#region RazorViewEngine
        //internal string GetModuleNameFromContext(ActionContext context)
        //{
        //    if (context != null && context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        //    {
        //        var type = Types.TypeHelpers.ExtractGenericInterface(controllerActionDescriptor.ControllerTypeInfo, typeof(Core.Modules.IModuleController<>));
        //        if (type != null)
        //        {
        //            return type.GenericTypeArguments[0].Namespace.Replace(typeof(OnWeb.Plugins.NamespaceAnchor).Namespace, "").TrimStart('.');
        //        }
        //    }

        //    return null;
        //}
        //ViewEngineResult IViewEngine.FindView(ActionContext context, string viewName, bool isMainPage)//. .F1indView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        //{
        //    if (context == null)
        //    {
        //        throw new ArgumentNullException("context");
        //    }
        //    if (string.IsNullOrEmpty(viewName))
        //    {
        //        throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, "viewName");
        //    }
        //    if (IsApplicationRelativePath(viewName) || IsRelativePath(viewName))
        //    {
        //        return ViewEngineResult.NotFound(viewName, Enumerable.Empty<string>());
        //    }
        //    ViewLocationCacheResult result = LocatePageFromViewLocations(context, viewName, isMainPage);
        //    return CreateViewEngineResult(result, viewName);
        //    //var masterNamePath = GetFilePath(GetModuleNameFromContext(context), masterName, true, out searchLocations);

        //    if (!string.IsNullOrEmpty(viewNamePath))
        //    {
        //        var res = new ViewEngineResult(CreateView(context, viewNamePath), this);
        //        return res;
        //    }

        //    var result = _previousViewEngine.FindView(context, viewName, isMainPage, isMainPage);
        //    return result;
        //}

        ////ViewEngineResult IViewEngine.FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        ////{
        ////    //Debug.WriteLine("FindPartialView: partialViewName={0}, useCache={1}", partialViewName, useCache);

        ////    var namesToSearch = new List<string>();

        ////    var extension = System.IO.Path.GetExtension(partialViewName);
        ////    if (string.IsNullOrEmpty(extension))
        ////    {
        ////        namesToSearch.Add(partialViewName + ".cshtml");
        ////        namesToSearch.Add(partialViewName + ".vbhtml");
        ////    }
        ////    else namesToSearch.Add(partialViewName);

        ////    foreach (var _partialViewName in namesToSearch)
        ////    {
        ////        var partialViewNamePath = GetFilePath(GetModuleNameFromContext(controllerContext), _partialViewName, true, out IEnumerable<string> searchLocations);

        ////        if (!string.IsNullOrEmpty(partialViewNamePath))
        ////        {
        ////            var res = new ViewEngineResult(CreatePartialView(controllerContext, partialViewNamePath), this);
        ////            return res;
        ////        }
        ////    }

        ////    return _previousViewEngine.FindPartialView(controllerContext, partialViewName, useCache);
        ////}

        ////void IViewEngine.ReleaseView(ControllerContext controllerContext, IView view)
        ////{
        ////    (view as IDisposable)?.Dispose();
        ////}

        //private IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        //{
        //    return null;// new RazorView(controllerContext, viewPath, masterPath, true, new string[] { "cshtml", "vbhtml" }, null);
        //}

        //private IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        //{
        //    return null;// new RazorView(controllerContext, partialPath, null, false, new string[] { "cshtml", "vbhtml" }, null);
        //}
        //#endregion

        //        #region IRouteHandler
        //        class Handler : IHttpHandler
        //        {
        //            public bool IsReusable
        //            {
        //                get { return true; }
        //            }

        //            public void ProcessRequest(HttpContext context)
        //            {
        //            }
        //        }

        //        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        //        {
        //            HttpContext.Current.Items["TimeStart"] = null;

        //            var fileRelative = "data/" + requestContext.RouteData.Values["filename"] as string;
        //            var fileReal = GetFilePath(null, fileRelative, false, out IEnumerable<string> searchLocations);

        //            requestContext.HttpContext.Response.Clear();

        //            if (fileReal == null)
        //            {
        //                //Debug.WriteLineNoLog("fileRelative unknown={0}", fileRelative);
        //                requestContext.HttpContext.Response.StatusCode = 404;
        //            }
        //            else
        //            {
        //                var ctt = System.Web.MimeMapping.GetMimeMapping(fileReal);
        //                requestContext.HttpContext.Response.ContentType = ctt;
        //                requestContext.HttpContext.Response.WriteFile(fileReal);
        //            }

        //            HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
        //            HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
        //            HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

        //            try
        //            {
        //                //requestContext.HttpContext.Response.End();
        //            }
        //            catch (ThreadAbortException) { }

        //            return new Handler();
        //        }
        //#endregion

    }
}

