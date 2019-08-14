using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OnUtils.Architecture.AppCore;
using System.Threading;

namespace OnWeb.CoreBind.Providers
{
    using System.Web.WebPages;
    using Core.Modules;

    class ResourceProvider : Core.Storage.ResourceProvider, IViewEngine, IRouteHandler, IVirtualPathFactory
    {
        private IVirtualPathFactory _previousPathFactory = null;
        private IViewEngine _previousViewEngine = null;
        private ThreadLocal<string> _currentModuleContext = new ThreadLocal<string>();
        private ThreadLocal<Dictionary<string, string>> _currentVirtualPathCache = new ThreadLocal<Dictionary<string, string>>(() => new Dictionary<string, string>());

        public ResourceProvider(IViewEngine previousViewEngine)
        {
            _previousViewEngine = previousViewEngine;
            if (_previousViewEngine is RazorViewEngine razorEngine)
            {
                var locationFormats = razorEngine.ViewLocationFormats.ToList();
                locationFormats.Insert(0, "{0}");
                razorEngine.ViewLocationFormats = locationFormats.ToArray();

                locationFormats = razorEngine.MasterLocationFormats.ToList();
                locationFormats.Insert(0, "{0}");
                razorEngine.MasterLocationFormats = locationFormats.ToArray();
            }
        }

        protected override void OnStartProvider()
        {
            var property = typeof(VirtualPathFactoryManager).GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Static);
            if (property != null)
            {
                var member = typeof(VirtualPathFactoryManager).GetField("_virtualPathFactories", BindingFlags.NonPublic | BindingFlags.Instance);
                if (member != null)
                {
                    var instance = (VirtualPathFactoryManager)property.GetValue(null);
                    var list = (LinkedList<IVirtualPathFactory>)member.GetValue(instance);
                    var factory = list.First.Value;
                    _previousPathFactory = factory;
                    VirtualPathFactoryManager.RegisterVirtualPathFactory(this);
                }
            }
        }

        #region RazorViewEngine
        internal string GetModuleNameFromContext(ControllerContext controllerContext)
        {
            if (controllerContext != null && controllerContext.Controller != null && controllerContext.Controller is ModuleControllerBase)
            {
                var module = (controllerContext.Controller as ModuleControllerBase).ModuleBase;
                if (module != null) return module.QueryType.Namespace.Replace(typeof(OnWeb.Modules.NamespaceAnchor).Namespace, "").TrimStart('.');
            }

            return null;
        }

        ViewEngineResult IViewEngine.FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            if (GetState() != CoreComponentState.Started) return null;

            //Debug.WriteLine("FindView: viewName={0}, masterName={1}, useCache={2}", viewName, masterName, useCache);
            _currentModuleContext.Value = GetModuleNameFromContext(controllerContext);

            var viewNamePath = GetFilePath(_currentModuleContext.Value, viewName, true, out IEnumerable<string> searchLocations);
            var masterNamePath = GetFilePath(_currentModuleContext.Value, masterName, true, out searchLocations);

            if (!string.IsNullOrEmpty(viewNamePath))
            {
                //var res = new ViewEngineResult(CreateView(controllerContext, viewNamePath, masterNamePath ?? masterName), this);
                var res = _previousViewEngine.FindView(controllerContext, viewNamePath, masterNamePath, false);
                return res;
            }

            var result = _previousViewEngine.FindView(controllerContext, viewName, masterName, useCache);
            return result;
        }

        ViewEngineResult IViewEngine.FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (GetState() != CoreComponentState.Started) return null;

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
            if (GetState() != CoreComponentState.Started) return;

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

        #region IVirtualPathFactory
        object IVirtualPathFactory.CreateInstance(string virtualPath)
        {
            if (GetState() != CoreComponentState.Started) return null;

            if (_currentVirtualPathCache.Value.TryGetValue(virtualPath, out var realPath))
            {
                return _previousPathFactory.CreateInstance(realPath);
            }
            return _previousPathFactory.CreateInstance(virtualPath);
        }

        bool IVirtualPathFactory.Exists(string virtualPath)
        {
            if (GetState() != CoreComponentState.Started) return false;
            if (virtualPath.EndsWith("_ViewStart.cshtml")) return false;

            if (_currentModuleContext.IsValueCreated)
            {
                var t = GetFilePath(_currentModuleContext.Value, virtualPath, true, out var searchLocations);
                _currentVirtualPathCache.Value[virtualPath] = t;
                if (!string.IsNullOrEmpty(t)) return true;
            }

            return _previousPathFactory.Exists(virtualPath);
        }
        #endregion

        #region IRouteHandler
        class Handler : IHttpHandler
        {
            public bool IsReusable
            {
                get => true;
            }

            public void ProcessRequest(HttpContext context)
            {
            }
        }

        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
        {
            if (GetState() != CoreComponentState.Started) return null;

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
