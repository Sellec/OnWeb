using OnUtils.Application;
using OnUtils.Data;
using System;

namespace OnWeb
{
    using Plugins.WebCoreModule;

    /// <summary>
    /// Ядро веб-приложения.
    /// </summary>
    public abstract class WebApplicationCore : ApplicationCore
    {
        class ConnectionStringResolver : IConnectionStringResolver
        {
            internal WebApplicationCore _core = null;

            string IConnectionStringResolver.ResolveConnectionStringForDataContext(Type[] entityTypes)
            {
                return _core.ConnectionString;
            }
        }

        private WebCoreConfiguration _configurationAccessor = null;

        /// <summary>
        /// </summary>
        public WebApplicationCore(string physicalApplicationPath, string connectionString) : base(physicalApplicationPath)
        {
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
