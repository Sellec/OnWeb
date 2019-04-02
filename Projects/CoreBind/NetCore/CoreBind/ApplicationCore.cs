using OnUtils.Application;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using System;

namespace OnWeb.CoreBind
{
    /// <summary>
    /// Ядро веб-приложения для ASP.Net Core.
    /// </summary>
    public sealed class ApplicationCore : Core.ApplicationCore
    {
        /// <summary>
        /// </summary>
        public ApplicationCore(string physicalApplicationPath, string connectionString) : base(physicalApplicationPath, connectionString)
        {
            OnUtils.Tasks.TasksManager.SetDefaultService(new OnUtils.Tasks.MomentalThreading.TasksService());
        }

        /// <summary>
        /// См. <see cref="AppCore{TAppCore}.OnBindingsRequired(IBindingsCollection{TAppCore})"/>.
        /// </summary>
        protected override void OnBindingsRequired(IBindingsCollection<Core.ApplicationCore> bindingsCollection)
        {
            base.OnBindingsRequired(bindingsCollection);

            //bindingsCollection.SetSingleton<Core.Storage.ResourceProvider, ResourceProvider>(() =>
            //{
            //    var viewEnginePrevious = ViewEngines.Engines.LastOrDefault(x => !(x is ResourceProvider));
            //    var instance = new ResourceProvider(viewEnginePrevious);
            //    ViewEngines.Engines.Insert(0, instance);
            //    return instance;
            //});
            //bindingsCollection.SetSingleton<Routing.Manager>();
        }

        /// <summary>
        /// См. <see cref="ApplicationBase{TSelfReference}.OnApplicationStart"/>.
        /// </summary>
        protected override void OnApplicationStart()
        {
            ///*
            //* Инициализация провайдера контроллеров.
            //*/
            //var controllerProvider = new TraceControllerProvider(ControllerBuilder.Current.GetControllerFactory());
            //controllerProvider.Start(this);
            //ControllerBuilder.Current.SetControllerFactory(controllerProvider);

            //ModelMetadataProviders.Current = new TraceModelMetadataProvider();
            //// todo для Model.ValidateModel ModelValidatorProviders.Providers.Add(new TraceModelValidatorProvider());

            ///*
            // * 
            // * */
            //ValueProviderFactories.Factories.Insert(0, new TraceJsonValueProviderFactory());

            ///*
            // * 
            // * */
            ////TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(FileManager.ClearExpired) + "_minutely1", Cron.MinuteInterval(1), () => FileManager.ClearExpired());
            //// todo TasksManager.SetTask(typeof(Core.Storage.IFileManager).FullName + "_" + nameof(Core.Storage.IFileManager.UpdateFileCount) + "_minutely5", Cron.MinuteInterval(5), () => FileManager.UpdateFileCount());

            //var d = HttpContext.Current;

            //OnRegisterRoutes();
            //OnRegisterBundles();
            //OnRegisterWebApi();
        }

        private void OnRegisterRoutes()
        {
            //var routes = RouteTable.Routes;

            //routes.LowercaseUrls = true;
            //routes.RouteExistingFiles = true;

            //routes.IgnoreRoute("ckeditor/{*pathInfo}");
            //routes.IgnoreRoute("ckfinder/{*pathInfo}");
            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.(.*)?" });

            //routes.Add("data1", new Route(
            //    "data/{*filename}",
            //    new RouteValueDictionary(new { controller = "AControllerThatDoesntExists" }),
            //    (ResourceProvider)Get<Core.Storage.ResourceProvider>()
            //));

            //var languageChecker = new Routing.LanguageRouteConstraint(Get<Core.Languages.Manager>().GetLanguages()
            //                                                                .Where(x => !string.IsNullOrEmpty(x.ShortName))
            //                                                                .Select(x => x.ShortName).ToArray());

            //var routingHandler = new Routing.RouteHandler(this);

            //routes.Add("RoutingTable", new Routing.RouteWithDefaults(
            //    this,
            //    "{*url}",
            //    true,
            //    new RouteValueDictionary(new { url = routingHandler }),
            //    new RouteValueDictionary(new { area = "unknown" }),
            //    routingHandler
            //));

            ////routes.Add("LanguageRoute", new Routing.RouteWithDefaults(
            ////    this,
            ////    "{language}/{controller}/{action}/{*url}",
            ////    false,
            ////    new RouteValueDictionary(new { language = languageChecker, controller = languageChecker, action = languageChecker }),
            ////    new RouteValueDictionary(new { area = Routing.AreaConstants.User }),
            ////    new MvcRouteHandler()
            ////));

            //routes.Add("DefaultRoute", new Routing.RouteWithDefaults(
            //    this,
            //    "{controller}/{action}/{*url}",
            //    true,
            //    new RouteValueDictionary(new { language = languageChecker, controller = languageChecker, action = languageChecker }),
            //    new RouteValueDictionary(new { area = Routing.AreaConstants.User }),
            //    new MvcRouteHandler()
            //));

            //routes.Add("AdminRoute1", new Routing.RouteWithDefaults(
            //    this,
            //    "admin/mnadmin/{controller}/{action}/{*url}",
            //    false,
            //    null,
            //    new RouteValueDictionary(new { area = Routing.AreaConstants.AdminPanel }),
            //    new MvcRouteHandler()
            //));

            //routes.Add("AdminRoute2", new Routing.RouteWithDefaults(
            //    this,
            //    "admin/madmin/{controller}/{action}/{*url}",
            //    false,
            //    null,
            //    new RouteValueDictionary(new { area = Routing.AreaConstants.AdminPanel }),
            //    new MvcRouteHandler()
            //));

        }

        private void OnRegisterBundles()
        {
        }

        private void OnRegisterWebApi()
        {
        }

        #region Свойства
        /// <summary>
        /// См. <see cref="Core.ApplicationCore.ServerUrl"/>.
        /// </summary>
        public override Uri ServerUrl
        {
            get { return _serverUrl; }
            set
            {
                _serverUrl = value;
                IsServerUrlHasBeenSet = true;
            }
        }

        /// <summary>
        /// Указывает, был ли адрес сервера уже однажды назначен, т.е. является ли текущее значение <see cref="ServerUrl"/> значением по-умолчанию.
        /// </summary>
        internal bool IsServerUrlHasBeenSet { get; private set; } = false;

        private Uri _serverUrl = new Uri("http://localhost");
        #endregion
    }
}
