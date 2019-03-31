using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;

namespace OnWeb.Core.Configuration
{
    using DB;
    using Modules;

    /// <summary>
    /// Предоставляет методы для управления настройками модуля типа <typeparamref name="TModule"/>. Позволяет получить, изменить и применить измененные настройки к модулю.
    /// </summary>
    /// <seealso cref="ModuleConfiguration{TModule}"/>
    public class ModuleConfigurationManipulator<TModule> : CoreComponentBase<ApplicationCore>, IComponentTransient<ApplicationCore>, IUnitOfWorkAccessor<CoreContext>
        where TModule : ModuleCore<TModule>
    {
        private TModule _module = null;
        internal ConfigurationValuesProvider _valuesProviderUsable = null;

        internal ModuleConfigurationManipulator(TModule module, ConfigurationValuesProvider valuesProviderUsable)
        {
            _module = module;
            _valuesProviderUsable = valuesProviderUsable;
        }

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion

        /// <summary>
        /// Возвращает объект типа <typeparamref name="TConfiguration"/>, содержащий параметры модуля <typeparamref name="TModule"/>.
        /// Возвращенный объект находится в режиме "только для чтения" - изменение параметров невозможно, попытка выполнить set вызовет <see cref="InvalidOperationException"/>.
        /// Все объекты конфигурации, созданные путем вызова этого метода, манипулируют одним набором значений. 
        /// То есть после изменения конфигурации путем вызова <see cref="ApplyConfiguration{TConfiguration}(TConfiguration)"/> новые значения автоматически доступны во всех ранее созданных в данном методе экземплярах конфигурации.
        /// </summary>
        /// <exception cref="InvalidOperationException">Возникает, если модуль <typeparamref name="TModule"/> не найден.</exception>
        public TConfiguration GetUsable<TConfiguration>() where TConfiguration : ModuleConfiguration<TModule>, new()
        {
            if (_valuesProviderUsable == null) _valuesProviderUsable = ((ModulesManager)AppCore.GetModulesManager()).CreateValuesProviderForModule(_module);

            var configuration = new TConfiguration() { _isReadonly = true,  _valuesProvider = _valuesProviderUsable };
            return configuration;
        }

        /// <summary>
        /// Возвращает объект типа <typeparamref name="TConfiguration"/>, содержащий настройки модуля <typeparamref name="TModule"/>.
        /// Изменение значений настроек в возвращенном объекте не влияет на непосредственно используемые в работе приложения значения. Для применения измененных значений см. <see cref="ApplyConfiguration{TConfiguration}(TConfiguration)"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Возникает, если модуль <typeparamref name="TModule"/> не найден.</exception>
        public TConfiguration GetEditable<TConfiguration>() where TConfiguration : ModuleConfiguration<TModule>, new()
        {
            var valuesProviderUsable = ((ModulesManager)AppCore.GetModulesManager()).CreateValuesProviderForModule(_module);

            var configuration = new TConfiguration() { _isReadonly = false, _valuesProvider = valuesProviderUsable };
            return configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TConfiguration"></typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public ApplyConfigurationResult ApplyConfiguration<TConfiguration>(TConfiguration configuration) 
            where TConfiguration : ModuleConfiguration<TModule>, new()
        {
            return ((ModulesManager)AppCore.GetModulesManager()).ApplyModuleConfiguration<TModule, TConfiguration>(configuration, this, _module);
        }
    }
}
