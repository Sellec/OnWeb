using OnUtils.Architecture.AppCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace OnWeb
{
    using CoreBind.Binders;
    using CoreBind.Providers;

    /// <summary>
    /// Представляет приложение ASP.NET, умеющее инициализировать OnWeb.
    /// </summary>
    public abstract class HttpApplicationBase : System.Web.HttpApplication
    {
        private static object SyncRootStart = new object();
        private static volatile int _instancesCount = 0;
        private static WebApplicationAspNetMvc _applicationCore = null;
        private static bool _applicationCoreStarted = false;
        private static Uri _urlFirst = null;
        private static Guid _unique = Guid.NewGuid();

        [ThreadStatic]
        internal Queue<IDisposable> _requestSpecificDisposables;

        /// <summary>
        /// Создает новый экземпляр приложения ASP.NET.
        /// </summary>
        public HttpApplicationBase()
        {
        }

        #region "Virtual"
        /// <summary>
        /// Вызывается во время запуска приложения до создания ядра приложения.
        /// </summary>
        protected virtual void OnBeforeApplicationStart()
        {
        }

        /// <summary>
        /// Вызывается во время запуска приложения после создания и инициализации ядра приложения.
        /// </summary>
        protected virtual void OnAfterApplicationStart()
        {
        }

        /// <summary>
        /// Вызывается во время начала обработки входящего запроса.
        /// </summary>
        protected virtual void OnBeginRequest()
        {
        }

        /// <summary>
        /// Вызывается после обработки входящего запроса.
        /// </summary>
        protected virtual void OnEndRequest()
        {
        }

        /// <summary>
        /// Вызывается при возникновении необработанной ошибки в приложении.
        /// </summary>
        protected virtual void OnError(Exception ex)
        {
        }

        /// <summary>
        /// Вызывается при остановке приложения до остановки ядра приложения.
        /// </summary>
        protected virtual void OnApplicationStopping()
        {

        }

        /// <summary>
        /// Вызывается при остановке приложения после остановки ядра приложения.
        /// </summary>
        protected virtual void OnApplicationStopped()
        {

        }

        /// <summary>
        /// Возвращает строку подключения для приложения.
        /// </summary>
        protected abstract string ConnectionString
        {
            get;
        }

        #endregion

        #region HttpApplication
        internal void Application_Start()
        {
            Debug.WriteLine($"Application_Start({_unique}, {GetType().AssemblyQualifiedName})");

            HtmlHelper.ClientValidationEnabled = true;

            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            GlobalFilters.Filters.Add(new External.ActionParameterAlias.ParameterAliasAttributeGlobal());

            ModelBinders.Binders.Add(typeof(JsonDictionary), new JsonDictionaryModelBinder());
            ModelBinders.Binders.DefaultBinder = new TraceModelBinder();

            lock (SyncRootStart)
            {
                if (_applicationCore == null)
                {
                    try
                    {
                        OnBeforeApplicationStart();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("OnApplicationStart: {0}", ex.Message);
                        throw;
                    }

                    var physicalApplicationPath = Server.MapPath("~");

                    _applicationCore = new WebApplicationAspNetMvc(physicalApplicationPath, ConnectionString);
                    _applicationCoreStarted = false;
                }
            }
        }

        /// <summary>
        /// Не убирать. Нужен для обмана Readonly-режима сессий, чтобы новые сессии создавались и записывались. Иначе будут меняться только существующие сессии.
        /// </summary>
        public void Session_OnStart()
        {
        }

        internal void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                var exception = Server.GetLastError();
                // todo _applicationCore.OnError(exception);

                if (exception is HttpRequestValidationException)
                {
                    Response.Clear();
                    Response.StatusCode = 200;
                    Response.Write(@"[html]");
                    Response.End();
                }

                Response.Filter = null;

                this.OnError(exception);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal void Application_BeginRequest(Object sender, EventArgs e)
        {
            var isFirstRequest = (bool?)Context.GetType().GetProperty("FirstRequest", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic)?.GetValue(Context);
            if (isFirstRequest.HasValue && isFirstRequest.Value) _urlFirst = Request.Url;

            lock (SyncRootStart)
            {
                if (_applicationCore == null)
                {
                    try
                    {
                        OnBeforeApplicationStart();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("OnApplicationStart: {0}", ex.Message);
                        throw;
                    }

                    var physicalApplicationPath = Server.MapPath("~");

                    _applicationCore = new WebApplicationAspNetMvc(physicalApplicationPath, ConnectionString);
                    _applicationCoreStarted = false;
                }

                if (!_applicationCore.IsServerUrlHasBeenSet && _urlFirst != null)
                    _applicationCore.ServerUrl = new UriBuilder(_urlFirst.Scheme, _urlFirst.Host, _urlFirst.Port).Uri;

                if (!_applicationCoreStarted)
                {
                    _applicationCore.Start();

                    try
                    {
                        OnAfterApplicationStart();
                        // todo     _applicationCore.OnApplicationAfterStartAfterUserEvent();
                        _applicationCoreStarted = true;
                    }
                    catch (Exception ex)
                    {
                        _applicationCore = null;
                        Debug.WriteLine("OnAfterApplicationStart: {0}", ex.Message);
                        throw;
                    }
                }
            }

            Core.WebUtils.QueryLogHelper.QueryLogEnabled = true;
            Context.Items["TimeRequestStart"] = DateTime.Now;

            HttpContext.Current.SetAppCore(_applicationCore);
            _applicationCore.GetUserContextManager().ClearCurrentUserContext();

            _requestSpecificDisposables = new Queue<IDisposable>();

            try
            {
                /*
                 * Попытка распарсить json из запроса в <see cref="Request.Form"/>.
                 * */

                if (Request.ContentType.IndexOf("application/json", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    if (Request.InputStream.CanRead)
                    {
                        try
                        {
                            if (Request.InputStream.CanSeek) Request.InputStream.Seek(0, SeekOrigin.Begin);
                            var body = Request.InputStream;
                            var encoding = Request.ContentEncoding;
                            var reader = new System.IO.StreamReader(body, encoding);
                            string s = reader.ReadToEnd();

                            var jsonRequestObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(s, new Newtonsoft.Json.JsonSerializerSettings() {
                                 Error = null,
                            });
                            if (jsonRequestObject != null && jsonRequestObject.Count > 0)
                            {
                                var oQuery = Request.Form;
                                oQuery = (NameValueCollection)Request.GetType().GetField("_form", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Request);
                                if (oQuery != null)
                                {
                                    var oReadable = oQuery.GetType().GetProperty("IsReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                                    oReadable.SetValue(oQuery, false, null);
                                    foreach (var p in jsonRequestObject) Request.Form[p.Key] = p.Value?.ToString();
                                    oReadable.SetValue(oQuery, true, null);
                                }
                            }
                        }
                        finally { if (Request.InputStream.CanSeek) Request.InputStream.Seek(0, SeekOrigin.Begin); }
                    }
                }
            }
            catch (ThreadAbortException) { throw; }
            catch { }

            IsCompressionEnabled = true;

            try
            {
                this.OnBeginRequest();
            }
            catch (ThreadAbortException) { throw; }
            catch (Exception ex) { Debug.WriteLine("OnBeginRequest: " + ex.Message); }

            var encodings = Request.Headers.Get("Accept-Encoding");
            if (IsCompressionEnabled && encodings != null)
            {
                // Check the browser accepts deflate or gzip (deflate takes preference)
                encodings = encodings.ToLower();
                if (encodings.Contains("deflate"))
                {
                    Response.Filter = new DeflateStream(Response.Filter, CompressionMode.Compress);
                    Response.AppendHeader("Content-Encoding", "deflate");
                    Response.AppendHeader("Vary", "Content-Encoding");
                }
                else if (encodings.Contains("gzip"))
                {
                    Response.Filter = new GZipStream(Response.Filter, CompressionMode.Compress);
                    Response.AppendHeader("Content-Encoding", "gzip");
                    Response.AppendHeader("Vary", "Content-Encoding");
                }
            }

        }

        internal void Application_AcquireRequestState(object sender, EventArgs e)
        {
            Context.Items["TimeRequestState"] = DateTime.Now;
            if (_applicationCore.GetState() == CoreComponentState.Started)
            {
                var context = _applicationCore.Get<SessionBinder>().RestoreUserContextFromRequest();
                if (context != null) _applicationCore.GetUserContextManager().SetCurrentUserContext(context);
            }
        }

        internal void Application_EndRequest(Object sender, EventArgs e)
        {
            Context.Items["TimeRequestEnd"] = DateTime.Now;

            var queries = Core.WebUtils.QueryLogHelper.GetQueries();
            if (queries.Count > 0)
            {
            }

            try
            {
                this.OnEndRequest();
            }
            catch (ThreadAbortException) { throw; }
            catch (Exception ex) { Debug.WriteLine("OnBeginRequest: " + ex.Message); }

            var requestSpecificDisposables = _requestSpecificDisposables;
            if (requestSpecificDisposables != null)
            {
                while (requestSpecificDisposables.Count > 0)
                {
                    var item = requestSpecificDisposables.Dequeue();
                    try
                    {
                        item.Dispose();
                    }
                    catch (Exception ex) { Debug.WriteLine("TraceHttpApplication.EndRequest Disposables: {0}", ex.Message); }
                }
            }
            _requestSpecificDisposables = null;

            TraceSessionStateProvider.SaveUnsavedSessionItem();

            if (_applicationCore.GetState() == CoreComponentState.Started)
            {
                _applicationCore.GetUserContextManager().ClearCurrentUserContext();
            }

            var queries2 = Core.WebUtils.QueryLogHelper.GetQueries();
            if (queries2.Count > 0)
            {
            }
            if (queries.Count > 0 && queries.Count != queries2.Count)
            {
            }

            Core.WebUtils.QueryLogHelper.QueryLogEnabled = false;
            Core.WebUtils.QueryLogHelper.ClearQueries();
        }

        internal void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            UpdateSessionCookieExpiration();
        }

        internal void Application_Disposed(Object sender, EventArgs e)
        {
            //try
            //{
            //    Debug.WriteLine($"Application_Disposed({_unique}, {GetType().AssemblyQualifiedName})");
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine($"Application_Disposed({_unique}, {GetType().AssemblyQualifiedName}): {ex.ToString()}");
            //}
        }

        internal void Application_End(Object sender, EventArgs e)
        {
            try { OnUtils.Tasks.TasksManager.DeleteAllTasks(); } catch { }
            try { OnApplicationStopping(); } catch { }

            try
            {
                var appCore = _applicationCore;
                _applicationCore = null;
                if (appCore.GetState() == CoreComponentState.Started) appCore.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping application core: {ex.ToString()}");
            }

            try { OnApplicationStopped(); } catch { }
            try { OnUtils.Tasks.TasksManager.DeleteAllTasks(); } catch { }
        }
        #endregion

        private void UpdateSessionCookieExpiration()
        {
            var httpContext = HttpContext.Current;
            var sessionState = httpContext?.Session;

            if (sessionState == null) return;

            var sessionStateSection = System.Configuration.ConfigurationManager.GetSection("system.web/sessionState") as System.Web.Configuration.SessionStateSection;
            var sessionCookie = httpContext.Response.Cookies[sessionStateSection?.CookieName ?? "ASP.NET_SessionId"];

            if (sessionCookie == null) return;

            sessionCookie.Expires = DateTime.Now.AddMinutes(sessionState.Timeout);
            sessionCookie.HttpOnly = true;
            sessionCookie.Value = sessionState.SessionID;
        }

        /// <summary>
        /// </summary>
        public sealed override void Init()
        {
            _instancesCount++;
            base.Init();
        }

        /// <summary>
        /// </summary>
        public sealed override void Dispose()
        {
            _instancesCount--;
            base.Dispose();
        }

        #region Свойства
        /// <summary>
        /// Возвращает количество экземпляров приложения, запущенных в данный момент.
        /// </summary>
        public static int InstancesCount
        {
            get => _instancesCount;
        }

        /// <summary>
        /// Возвращает ядро приложения.
        /// </summary>
        public WebApplication AppCore
        {
            get => _applicationCore;
        }

        /// <summary>
        /// Указывает, следует ли использовать gzip/deflate, если браузер поддерживает сжатие.
        /// </summary>
        public bool IsCompressionEnabled
        {
            get;
            set;
        }
        #endregion

    }
}
