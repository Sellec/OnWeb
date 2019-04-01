using OnUtils.Architecture.AppCore;
using System;

namespace OnWeb.Core.Configuration
{
    /// <summary>
    /// Предоставляет доступ к настройкам модуля <typeparamref name="TModule"/>. 
    /// </summary>
    /// <typeparam name="TModule">Должен быть query-типом модуля, зарегистрированным в привязках типов.</typeparam>
    /// <seealso cref="AppCore{TAppCore}.GetQueryTypes"/>
    /// <seealso cref="ModuleConfigurationManipulator{TModule}"/>
    public class ModuleConfiguration<TModule>
        where TModule : Modules.ModuleCore<TModule>
    {
        internal bool _isReadonly = false;
        internal ConfigurationValuesProvider _valuesProvider = new ConfigurationValuesProvider();

        #region Dictionary
        /// <summary>
        /// Проверяет, существует ли параметр с ключом <paramref name="key"/>.
        /// </summary>
        public bool ContainsKey(string key)
        {
            return _valuesProvider.ContainsKey(key);
        }
        #endregion

        #region Получение значений
        /// <summary>
        /// Возвращает значение параметра с ключом <paramref name="key"/>. 
        /// Если ключ не существует, то возвращает <paramref name="defaultValue"/>.
        /// Значение с ключом <paramref name="key"/> автоматически приводится к типу <typeparamref name="T"/>. 
        /// Если попытка приведения оказывается неудачной, то возвращает <paramref name="defaultValue"/>.
        /// </summary>
        public T Get<T>(string key, T defaultValue)
        {
            return _valuesProvider.Get<T>(key, defaultValue);
        }

        /// <summary>
        /// Возвращает значение параметра с ключом <paramref name="key"/>. 
        /// Если ключ не существует, то возвращает значение по-умолчанию для типа <typeparamref name="T"/>.
        /// Значение с ключом <paramref name="key"/> автоматически приводится к типу <typeparamref name="T"/>. 
        /// Если попытка приведения оказывается неудачной, то возвращает значение по-умолчанию для типа <typeparamref name="T"/>.
        /// </summary>
        public T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }

        /// <summary>
        /// Задает значение параметра с ключом <paramref name="key"/>. 
        /// </summary>
        /// <exception cref="InvalidOperationException">Возникает, если объект конфигурации относится к используемым. Подразумевается, что запрещено менять значения параметров в объектах конфигурации модуля и ядра, доступных через <see cref="ApplicationCore.Config"/> и <see cref="Modules.ModuleCore{TSelfReference}.GetConfiguration{TConfiguration}"/>. Для изменения конфигурации следует пользоваться <see cref="ModuleConfigurationManipulator{TModule}.ApplyConfiguration{TConfiguration}(TConfiguration)"/>.</exception>
        public void Set<T>(string key, T value)
        {
            if (_isReadonly) throw new InvalidOperationException("Изменять значения параметров конфигурации запрещено.");
            _valuesProvider.Set(key, value);
        }

        #endregion

        #region Свойства
        /// <summary>
        /// Возвращает или задает url-доступное имя модуля.
        /// </summary>
        /// <seealso cref="Modules.ModuleCoreAttribute.DefaultUrlName"/>
        /// <seealso cref="Modules.ModuleCore.UrlName"/>
        public string UrlName
        {
            get => Get<string>("UrlName");
            set => Set<string>("UrlName", value);
        }
        #endregion
    }
}
