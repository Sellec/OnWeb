using OnUtils;
using OnUtils.Application;
using OnUtils.Application.Modules;
using OnUtils.Application.Users;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;
using OnUtils.Data;
using System;

namespace OnWeb
{
    using Core.Configuration;

    /// <summary>
    /// Ядро веб-приложения.
    /// </summary>
    public abstract class ApplicationCore : ApplicationBase<ApplicationCore>
    {
        class ConnectionStringResolver : IConnectionStringResolver
        {
            internal ApplicationCore _core = null;

            string IConnectionStringResolver.ResolveConnectionStringForDataContext(Type[] entityTypes)
            {
                return _core.ConnectionString;
            }
        }

        private CoreConfiguration _configurationAccessor = null;

        /// <summary>
        /// </summary>
        public ApplicationCore(string physicalApplicationPath, string connectionString)
        {
            try
            {
                ConnectionString = connectionString;
                LibraryEnumeratorFactory.LibraryDirectory = physicalApplicationPath;
                ApplicationWorkingFolder = physicalApplicationPath;
                DataAccessManager.SetConnectionStringResolver(new ConnectionStringResolver() { _core = this });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error init ApplicationCore: {0}", ex.ToString());
                if (ex.InnerException != null) Debug.WriteLine("Error init ApplicationCore inner: {0}", ex.InnerException.Message);
                if (ex.InnerException?.InnerException != null) Debug.WriteLine("Error init ApplicationCore inner inner: {0}", ex.InnerException.InnerException.Message);
                if (ex.InnerException?.InnerException?.InnerException != null) Debug.WriteLine("Error init ApplicationCore inner inner inner: {0}", ex.InnerException.InnerException.InnerException.Message);

                throw;
            }
        }

        #region Методы
        /// <summary>
        /// См. <see cref="ApplicationBase{TSelfReference}.OnApplicationStart"/>.
        /// </summary>
        protected override void OnApplicationStart()
        {
        }

        /// <summary>
        /// См. <see cref="AppCore{TAppCore}.OnBindingsApplied"/>.
        /// </summary>
        protected sealed override void OnBindingsApplied()
        {
        }

        /// <summary>
        /// См. <see cref="AppCore{TAppCore}.OnBindingsRequired(IBindingsCollection{TAppCore})"/>.
        /// </summary>
        protected override void OnBindingsRequired(IBindingsCollection<ApplicationCore> bindingsCollection)
        {
            bindingsCollection.SetSingleton<Core.Addresses.IManager, Core.Addresses.AddressManager>();
            bindingsCollection.SetSingleton<Core.Items.ItemsManager>();
            bindingsCollection.SetSingleton<Core.Journaling.IManager, Core.Journaling.Manager>();
            bindingsCollection.SetSingleton<Core.Messaging.IMessagingManager, Core.Messaging.MessagingManager>();
            bindingsCollection.SetSingleton<Core.Languages.Manager>();
            bindingsCollection.SetSingleton<ModulesManager<ApplicationCore>, Core.Modules.ModulesManager>();
            bindingsCollection.SetSingleton<Core.Modules.ModulesLoadStarter>();
            bindingsCollection.SetSingleton<Core.Routing.UrlManager>();
            bindingsCollection.SetSingleton<Core.ServiceMonitor.Monitor>();
            bindingsCollection.SetSingleton<Core.Users.IEntitiesManager, Core.Users.EntitiesManager>();
            bindingsCollection.SetSingleton<UserContextManager<ApplicationCore>, Core.Users.UserContextManager>();
            bindingsCollection.SetSingleton<Core.Users.IUsersManager, Core.Users.UsersManager>();
        }

        private void OnApplicationStartBase()
        {

        }

        /// <summary>
        /// См. <see cref="AppCore{TAppCore}.OnInstanceActivated{TRequestedType}(IComponent{TAppCore})"/>.
        /// </summary>
        protected override void OnInstanceActivated<TRequestedType>(IComponent<ApplicationCore> instance)
        {
         
        }
        #endregion

        #region Упрощение доступа
        /// <summary>
        /// Возвращает менеджер модулей для приложения.
        /// </summary>
        public ModulesManager<ApplicationCore> GetModulesManager()
        {
            return Get<ModulesManager<ApplicationCore>>();
        }

        /// <summary>
        /// Возвращает менеджер контекстов пользователя для приложения.
        /// </summary>
        public Core.Users.UserContextManager GetUserContextManager()
        {
            return (Core.Users.UserContextManager)Get<UserContextManager<ApplicationCore>>();
        }

        private Plugins.ModuleCore.Module GetCoreModule()
        {
            return GetModulesManager().GetModule<Plugins.ModuleCore.Module>();
        }

        #endregion

        #region Управление настройками.
        /// <summary>
        /// Возвращает значение конфигурационной опции. Если значение не найдено, то возвращается <paramref name="defaultValue"/>.
        /// </summary>
        public T ConfigurationOptionGet<T>(string name, T defaultValue = default(T))
        {
            if (Config.ContainsKey(name))
            {
                return Config.Get<T>(name, defaultValue);
            }
            return defaultValue;
        }
        #endregion

        #region Свойства


        /// <summary>
        /// Основные настройки сайта.
        /// </summary>
        public CoreConfiguration Config
        {
            get
            {
                if (_configurationAccessor == null) _configurationAccessor = GetCoreModule().GetConfiguration<CoreConfiguration>();
                return _configurationAccessor;
            }
        }

        /// <summary>
        /// Внешний URL-адрес сервера.
        /// </summary>
        public virtual Uri ServerUrl
        {
            get;
            set;
        } = new Uri("http://localhost");

        /// <summary>
        /// Возвращает рабочую директорию приложения. 
        /// </summary>
        public string ApplicationWorkingFolder { get; private set; }

        /// <summary>
        /// Возвращает строку подключения. todo - разобраться со строками подключений.
        /// </summary>
        public string ConnectionString { get; private set; }

        #endregion
    }
}
