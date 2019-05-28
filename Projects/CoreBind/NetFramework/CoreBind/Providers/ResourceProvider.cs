using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnWeb.CoreBind.Providers
{
    class ResourceProvider : Core.Storage.ResourceProvider, IViewEngine, IRouteHandler
    {
        private IViewEngine _previousViewEngine = null;

        public ResourceProvider(IViewEngine previousViewEngine)
        {
            _previousViewEngine = previousViewEngine;
        }

        #region RazorViewEngine
        internal string GetModuleNameFromContext(ControllerContext controllerContext)
        {
            if (controllerContext != null && controllerContext.Controller != null && controllerContext.Controller is Modules.ModuleControllerBase)
            {
                var module = (controllerContext.Controller as Modules.ModuleControllerBase).ModuleBase;
                if (module != null) return module.GetType().Namespace.Replace(typeof(OnWeb.Plugins.NamespaceAnchor).Namespace, "").TrimStart('.');
            }

            return null;
        }

        ViewEngineResult IViewEngine.FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            //Debug.WriteLine("FindView: viewName={0}, masterName={1}, useCache={2}", viewName, masterName, useCache);

            var viewNamePath = GetFilePath(GetModuleNameFromContext(controllerContext), viewName, true, out IEnumerable<string> searchLocations);
            var masterNamePath = GetFilePath(GetModuleNameFromContext(controllerContext), masterName, true, out searchLocations);

            if (!string.IsNullOrEmpty(viewNamePath))
            {
                var res = new ViewEngineResult(CreateView(controllerContext, viewNamePath, masterNamePath ?? masterName), this);
                return res;
            }
            
            var result = _previousViewEngine.FindView(controllerContext, viewName, masterName, useCache);
            return result;
        }

        ViewEngineResult IViewEngine.FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            //Debug.WriteLine("FindPartialView: partialViewName={0}, useCache={1}", partialViewName, useCache);

            var namesToSearch = new List<string>();

            var extension = System.IO.Path.GetExtension(partialViewName);
            if (string.IsNullOrEmpty(extension))
            {
                namesToSearch.Add(partialViewName + ".cshtml");
                namesToSearch.Add(partialViewName + ".vbhtml");
            }
            else namesToSearch.Add(partialViewName);

            foreach (var _partialViewName in namesToSearch)
            {
                var partialViewNamePath = GetFilePath(GetModuleNameFromContext(controllerContext), _partialViewName, true, out IEnumerable<string> searchLocations);

                if (!string.IsNullOrEmpty(partialViewNamePath))
                {
                    var res = new ViewEngineResult(CreatePartialView(controllerContext, partialViewNamePath), this);
                    return res;
                }
            }

            return _previousViewEngine.FindPartialView(controllerContext, partialViewName, useCache);
        }

        void IViewEngine.ReleaseView(ControllerContext controllerContext, IView view)
        {
            (view as IDisposable)?.Dispose();
        }

        private IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            return new RazorView(controllerContext, viewPath, masterPath, true, new string[] { "cshtml", "vbhtml" }, null);
        }

        private IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            return new RazorView(controllerContext, partialPath, null, false, new string[] { "cshtml", "vbhtml" }, null);
        }
        #endregion

        #region IRouteHandler
        class Handler : IHttpHandler
        {
            public bool IsReusable
            {
                get { return true; }
            }

            public void ProcessRequest(HttpContext context)
            {
            }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            HttpContext.Current.Items["TimeStart"] = null;

            var fileRelative = "data/" + requestContext.RouteData.Values["filename"] as string;
            var fileReal = GetFilePath(null, fileRelative, false, out IEnumerable<string> searchLocations);

            return GetHttpHandler(requestContext, fileReal);
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext, string fileReal)
        {
            requestContext.HttpContext.Response.Clear();

            if (fileReal == null)
            {
                requestContext.HttpContext.Response.StatusCode = 404;
            }
            else
            {
                var ctt = MimeMapping.GetMimeMapping(fileReal);
                requestContext.HttpContext.Response.ContentType = ctt;
                requestContext.HttpContext.Response.TransmitFile(fileReal);
            }

            //todo эти две строки ломают сжатие gzip, если его включить.
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.SuppressContent = true;

            HttpContext.Current.ApplicationInstance.CompleteRequest();

            return new Handler();
        }
#endregion

        #region Прекомпиляция
        [System.Diagnostics.DebuggerNonUserCode]
        public void GeneratePrecompiled(bool logOnlyErrors = true)
        {
            logOnlyErrors = true;

            var filesList = new List<string>();

            var field = typeof(FileInfo).GetField("FullPath", BindingFlags.Instance | BindingFlags.NonPublic);

            var paths = SourceDevelopmentPathList.Merge(SourcePathList);

            foreach (var path in paths)
            {
                if (!Directory.Exists(path)) continue;

                var fileList = new DirectoryInfo(path).GetFiles("*.cshtml", SearchOption.AllDirectories);
                foreach (var file in fileList)
                {
                    try
                    {
                        var filepath = "";
                        if (field != null) filepath = (string)field.GetValue(file);
                        else filepath = file.FullName;

                        if (filepath.ToLower().Contains("symlink")) continue;
                        if (!filesList.Contains(filepath) && !filepath.Contains("\\bin\\")) filesList.Add(filepath);
                    }
                    catch { }
                }
            }

            var measure = new MeasureTime();
            try
            {
                foreach (var file in filesList)
                {
                    var relPath = TranslateFullPathTo(file);

                    if (Path.GetFileName(relPath).ToLower().StartsWith("base")) continue;

                    GeneratePrecompiledView(relPath, logOnlyErrors);
                }

                Debug.WriteLine("Precompiling Background {0} files, time {1:D}ms. See detailed errors in console", filesList.Count, measure);
            }
            catch (Exception ex) { Debug.WriteLine("Precompiling Background {0} files, time {1:D}ms with error: {2}. See detailed errors in console", filesList.Count, measure, ex.Message); }

            //Task.Factory.StartNew(() =>
            //{
            //    var measure = new MeasureTime();
            //    try
            //    {
            //        var tasks = new List<Task>();
            //        foreach (var file in filesList)
            //        {
            //            var relPath = TranslateFullPathTo(file);

            //            if (Path.GetFileName(relPath).ToLower().StartsWith("base"))
            //                continue;

            //            tasks.Add(Task.Factory.StartNew((state) =>
            //            {
            //                var _relPath = state as string;
            //                GeneratePrecompiledView(_relPath, logOnlyErrors);
            //            }, relPath, TaskCreationOptions.AttachedToParent));
            //        }
            //        Task.WaitAll(tasks.ToArray());

            //        Debug.WriteLine("Precompiling Background {0} files, time {1:D}ms", filesList.Count, measure);
            //    }
            //    catch (Exception ex) { Debug.WriteLine("Precompiling Background {0} files, time {1:D}ms with error: {2}", filesList.Count, measure, ex.Message); }
            //}, TaskCreationOptions.LongRunning);
        }

        [System.Diagnostics.DebuggerStepThrough]
        private void GeneratePrecompiledView(string _relPath, bool logOnlyErrors = true)
        {
            var measure2 = new MeasureTime();
            var message = "";

            var success = false;
            try
            {
                var id = System.Threading.Thread.CurrentThread.ManagedThreadId;

                measure2.Start();
                var type = System.Web.Compilation.BuildManager.GetCompiledType(_relPath);
                success = true;
            }
            catch (Exception ex)
            {
                var exx = ex.GetLowLevelException();
                message = ", err: " + exx.Message;
            }
            finally
            {
                if (!logOnlyErrors || (logOnlyErrors && !string.IsNullOrEmpty(message)))
                    Debug.WriteLineNoLog("Precompiling '{0}' is {1}, time {2:D}ms {3}", _relPath, success, measure2, message);
            }
        }
        #endregion
    }
}

