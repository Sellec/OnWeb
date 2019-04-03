using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Razor
{
    public class HttpApplication : System.Web.HttpApplication
    {
        private static object SyncRootStart = new object();
        private static volatile bool _initialized = false;
        private static volatile int _instancesCount = 0;
        private static ApplicationCoreBind _applicationCore = null;

        [ThreadStatic]
        internal Queue<IDisposable> _requestSpecificDisposables;

        public HttpApplication()
        {
        }

        #region "Virtual"
        protected virtual void OnBeforeApplicationStart()
        {
        }

        protected virtual void OnAfterApplicationStart()
        {
        }

        protected virtual void OnBeginRequest()
        {
        }

        protected virtual void OnEndRequest()
        {
        }

        protected virtual void OnError(Exception ex)
        {
        }

        protected virtual string ConnectionString
        {
            get
            {
                return "";
            }
        }

        #endregion

        #region HttpApplication
        internal void Application_Start()
        {
            HtmlHelper.ClientValidationEnabled = true;

            GlobalFilters.Filters.Add(new HandleErrorAttribute());

            ModelBinders.Binders.Add(typeof(Binders.JsonDictionary), new Binders.JsonDictionaryModelBinder());
            ModelBinders.Binders.DefaultBinder = new Binders.TraceModelBinder();

            lock (SyncRootStart)
                if (_applicationCore == null)
                {
                    try
                    {
                        this.OnBeforeApplicationStart();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("OnApplicationStart: {0}", ex.Message);
                        throw;
                    }

                    var physicalApplicationPath = Server.MapPath("~");

                    _applicationCore = new ApplicationCoreBind(physicalApplicationPath, ConnectionString);
#pragma warning disable CS0612
                    ApplicationCoreSingleton.Instance = _applicationCore;
#pragma warning restore CS0612
                    _applicationCore.Start();

                    try
                    {
                        this.OnAfterApplicationStart();
                    // todo     _applicationCore.OnApplicationAfterStartAfterUserEvent();
                    }
                    catch (Exception ex)
                    {
                        _applicationCore = null;
                        Debug.WriteLine("OnAfterApplicationStart: {0}", ex.Message);
                        throw;
                    }

                    _initialized = true;
                }
        }

        //Не убирать. Нужен для обмана Readonly-режима сессий, чтобы новые сессии создавались и записывались. Иначе будут меняться только существующие сессии.
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

                this.OnError(exception);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        internal void Application_BeginRequest(Object sender, EventArgs e)
        {
            var isFirstRequest = (bool?)this.Context.GetType().GetProperty("FirstRequest", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic)?.GetValue(this.Context);
            if (isFirstRequest.HasValue && isFirstRequest.Value == true)
            {
                Debug.WriteLine($"RequestUrl='{this.Request.Url?.ToString()}', serverUrl='{_applicationCore.ServerUrl?.ToString()}'");
            }
            if (!_applicationCore.IsServerUrlHasBeenSet  && isFirstRequest.HasValue && isFirstRequest.Value)
                _applicationCore.ServerUrl = new UriBuilder(Request.Url.Scheme, Request.Url.Host, Request.Url.Port).Uri;

            _applicationCore.GetUserContextManager().SetCurrentUserContext(_applicationCore.GetUserContextManager().CreateGuestUserContext());

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

            try
            {
                this.OnBeginRequest();
            }
            catch (ThreadAbortException) { throw; }
            catch (Exception ex) { Debug.WriteLine("OnBeginRequest: " + ex.Message); }
        }

        internal void Application_EndRequest(Object sender, EventArgs e)
        {
            try
            {
                this.OnEndRequest();
            }
            catch (ThreadAbortException) { throw; }
            catch (Exception ex) { Debug.WriteLine("OnBeginRequest: " + ex.Message); }

            while (_requestSpecificDisposables.Count > 0)
            {
                var item = _requestSpecificDisposables.Dequeue();
                try
                {
                    item.Dispose();
                }
                catch (Exception ex) { Debug.WriteLine("TraceHttpApplication.EndRequest Disposables: {0}", ex.Message); }
            }
            _requestSpecificDisposables = null;

            Providers.TraceSessionStateProvider.SaveUnsavedSessionItem();
        }

        public void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            UpdateSessionCookieExpiration();
        }

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

        public sealed override void Init()
        {
            _instancesCount++;
            base.Init();
        }

        public sealed override void Dispose()
        {
            _instancesCount--;
            base.Dispose();
        }
        #endregion

        #region Свойства
        /// <summary>
        /// Возвращает количество экземпляров приложения, запущенных в данный момент.
        /// </summary>
        public static int InstancesCount
        {
            get { return _instancesCount; }
        }

        /// <summary>
        /// Возвращает ядро приложения.
        /// </summary>
        public ApplicationCore AppCore
        {
            get => _applicationCore;
        }
        #endregion

    }
}
