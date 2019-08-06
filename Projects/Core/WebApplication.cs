using OnUtils.Application;
using OnUtils.Data;
using System;
using OnUtils.Application.Modules.CoreModule;

namespace OnWeb
{
    using Plugins.WebCoreModule;

    /// <summary>
    /// Ядро веб-приложения.
    /// Не предполагается создание пользовательских экземпляров приложения, следует пользоваться абстрактной версией для создания ссылок.
    /// </summary>
    public abstract class WebApplication : ApplicationCore<WebApplication>
    {
        class ConnectionStringResolver : IConnectionStringResolver
        {
            internal WebApplication _core = null;

            string IConnectionStringResolver.ResolveConnectionStringForDataContext(Type[] entityTypes)
            {
                return _core.ConnectionString;
            }
        }

        private WebCoreConfiguration _configurationAccessor = null;

        /// <summary>
        /// </summary>
        public WebApplication(string physicalApplicationPath, string connectionString) : base(physicalApplicationPath)
        {
            if (!GetType().Assembly.FullName.EndsWith("")) throw new InvalidProgramException("");

            try
            {
                ConnectionString = connectionString;
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

        #region Свойства
        /// <summary>
        /// Возвращает модуль ядра приложения.
        /// </summary>
        public CoreModule<WebApplication> AppCoreModule
        {
            get => Get<CoreModule<WebApplication>>();
        }

        /// <summary>
        /// Возвращает основной веб-модуль приложения.
        /// </summary>
        public WebCoreModule WebCoreModule
        {
            get => Get<WebCoreModule>();
        }

        /// <summary>
        /// Основные настройки веб-приложения.
        /// </summary>
        public WebCoreConfiguration WebConfig
        {
            get
            {
                if (_configurationAccessor == null) _configurationAccessor = Get<WebCoreModule>().GetConfiguration<WebCoreConfiguration>();
                return _configurationAccessor;
            }
        }

        /// <summary>
        /// Возвращает строку подключения. todo - разобраться со строками подключений.
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Внешний URL-адрес сервера.
        /// </summary>
        public virtual Uri ServerUrl
        {
            get;
            set;
        } = new Uri("http://localhost");
        #endregion
    }
}
