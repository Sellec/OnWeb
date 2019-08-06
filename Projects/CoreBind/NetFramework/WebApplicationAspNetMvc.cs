using OnUtils.Application;
using OnUtils.Application.Users;
using OnUtils.Application.Languages;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnWeb
{
    using Core;
    using CoreBind.Providers;
    using CoreBind.Routing;

    /// <summary>
    /// Ядро веб-приложения для ASP.Net MVC.
    /// </summary>
    sealed class WebApplicationAspNetMvc : WebApplication
    {
        /// <summary>
        /// </summary>
        public WebApplicationAspNetMvc(string physicalApplicationPath, string connectionString) : base(physicalApplicationPath, connectionString)
        {
            OnUtils.Tasks.TasksManager.SetDefaultService(new OnUtils.Tasks.MomentalThreading.TasksService());
        }

        /// <summary>
        /// См. <see cref="AppCore{TAppCore}.OnBindingsRequired(IBindingsCollection{TAppCore})"/>.
        /// </summary>
        protected override void OnBindingsRequired(IBindingsCollection<WebApplication> bindingsCollection)
        {
            base.OnBindingsRequired(bindingsCollection);

            bindingsCollection.SetSingleton<Core.Storage.ResourceProvider, ResourceProvider>(() =>
            {
                var viewEnginePrevious = ViewEngines.Engines.LastOrDefault(x => !(x is ResourceProvider));
                var instance = new ResourceProvider(viewEnginePrevious);
                ViewEngines.Engines.Insert(0, instance);
                return instance;
            });
            bindingsCollection.SetSingleton<RoutingManager>();
            bindingsCollection.SetSingleton<UserContextManager<WebApplication>, CoreBind.Users.UserContextManager>();
        }

        /// <summary>
        /// См. <see cref="ApplicationCore{TAppCoreSelfReference}.OnApplicationStart"/>.
        /// </summary>
        protected override void OnApplicationStart()
        {
            /*
            * Инициализация провайдера контроллеров.
            */
            var controllerProvider = new TraceControllerProvider(ControllerBuilder.Current.GetControllerFactory());
            ((IComponentStartable)controllerProvider).Start(this);
            ControllerBuilder.Current.SetControllerFactory(controllerProvider);

            if (!(ModelMetadataProviders.Current is TraceModelMetadataProvider))
                ModelMetadataProviders.Current = new TraceModelMetadataProvider();

            // todo для Model.ValidateModel ModelValidatorProviders.Providers.Add(new TraceModelValidatorProvider());

            /*
             * 
             * */
            ValueProviderFactories.Factories.Insert(0, new TraceJsonValueProviderFactory());

            /*
             * 
             * */
            //TasksManager.SetTask(typeof(FileManager).FullName + "_" + nameof(FileManager.ClearExpired) + "_minutely1", Cron.MinuteInterval(1), () => FileManager.ClearExpired());
            // todo TasksManager.SetTask(typeof(Core.Storage.IFileManager).FullName + "_" + nameof(Core.Storage.IFileManager.UpdateFileCount) + "_minutely5", Cron.MinuteInterval(5), () => FileManager.UpdateFileCount());

            var d = HttpContext.Current;

            OnRegisterRoutes();
            OnRegisterBundles();
            OnRegisterWebApi();
        }

        private void OnRegisterRoutes()
        {
            var routes = RouteTable.Routes;

            routes.LowercaseUrls = true;
            routes.RouteExistingFiles = true;

            routes.IgnoreRoute("ckeditor/{*pathInfo}");
            routes.IgnoreRoute("ckfinder/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.(.*)?" });

            routes.Add("data1", new Route(
                "data/{*filename}",
                new RouteValueDictionary(new { controller = "AControllerThatDoesntExists" }),
                (ResourceProvider)Get<Core.Storage.ResourceProvider>() 
            ));

            var languageChecker = new LanguageRouteConstraint(Get<Manager<WebApplication>>().GetLanguages()
                                                                            .Where(x => !string.IsNullOrEmpty(x.ShortName))
                                                                            .Select(x => x.ShortName).ToArray());

            var routingHandler = new RouteHandler(this);

            // Маршруты админки идут перед универсальными.
            var moduleAdmin = Get<Plugins.Admin.ModuleAdmin>();

            routes.Add("AdminRoute1", new RouteWithDefaults(
                this,
                moduleAdmin.UrlName + "/mnadmin/{controller}/{action}/{*url}",
                false,
                null,
                new RouteValueDictionary(new { area = AreaConstants.AdminPanel }),
                new MvcRouteHandler()
            ));

            routes.Add("AdminRoute2", new RouteWithDefaults(
                this,
                moduleAdmin.UrlName + "/madmin/{controller}/{action}/{*url}",
                false,
                null,
                new RouteValueDictionary(new { area = AreaConstants.AdminPanel }),
                new MvcRouteHandler()
            ));

            // Универсальные маршруты.
            routes.Add("RoutingTable", new RouteWithDefaults(
                this,
                "{*url}",
                true,
                new RouteValueDictionary(new { url = routingHandler }),
                new RouteValueDictionary(new { area = "unknown" }),
                routingHandler
            ));

            //routes.Add("LanguageRoute", new Routing.RouteWithDefaults(
            //    this,
            //    "{language}/{controller}/{action}/{*url}",
            //    false,
            //    new RouteValueDictionary(new { language = languageChecker, controller = languageChecker, action = languageChecker }),
            //    new RouteValueDictionary(new { area = Routing.AreaConstants.User }),
            //    new MvcRouteHandler()
            //));

            routes.Add("DefaultRoute", new RouteWithDefaults(
                this,
                "{controller}/{action}/{*url}",
                true,
                new RouteValueDictionary(new { language = languageChecker, controller = languageChecker, action = languageChecker }),
                new RouteValueDictionary(new { area = AreaConstants.User }),
                new MvcRouteHandler()
            ));

        }

        private void OnRegisterBundles()
        {
        }

        private void OnRegisterWebApi()
        {
        }

        #region Свойства
        /// <summary>
        /// См. <see cref="WebApplication.ServerUrl"/>.
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
        public bool IsServerUrlHasBeenSet { get; private set; } = false;

        private Uri _serverUrl = new Uri("http://localhost");
        #endregion
    }
}
