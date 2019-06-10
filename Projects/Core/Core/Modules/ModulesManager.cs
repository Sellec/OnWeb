using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using OnUtils;
using OnUtils.Application;
using OnUtils.Application.Modules;
using OnUtils.Application.Users;
using OnUtils.Data;
using OnUtils.Data.UnitOfWork;
using OnUtils.Utils;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Core.Modules
{
    using Configuration;
    using DB;

    // todo ModulesManager привести в порядок описание методов, методы и тело в ModulesManager, сделать общий подход как в FileManager.
    /// <summary>
    /// Менеджер, управляющий модулями системы.
    /// Система разделена на модули с определенным функционалом, к модулям могут быть привязаны операции, доступные пользователю извне (для внешних запросов).
    /// Права доступа регистрируются на модуль.
    /// </summary>
    class ModulesManager : ModulesManager<ApplicationCore>, IUnitOfWorkAccessor<CoreContext>
    {
        /// <summary>
        /// Создает новый экземпляр менеджера модулей.
        /// </summary>
        public ModulesManager() : base(false)
        {
            DeprecatedSingletonInstances.ModulesManager = this;
            // todo this.RegisterJournal("Менеджер модулей");
        }

        #region Методы
        internal new void StartModules()
        {
            base.StartModules();
        }

        protected override bool FilterModuleTypes(Type typeFromDI)
        {
            var moduleCoreType = typeof(ModuleCore);
            return moduleCoreType.IsAssignableFrom(typeFromDI) && typeFromDI.GetCustomAttribute< ModuleCoreAttribute>() != null;
        }

        protected override void LoadModule<TModuleType>(TModuleType module)
        {
            this.GetType().GetMethod(nameof(LoadModuleCustom), BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(typeof(TModuleType)).Invoke(this, new object[] { module });
        }

        protected void LoadModuleCustom<TModuleType>(TModuleType module) where TModuleType : ModuleCore<TModuleType>
        {
            var moduleType = typeof(TModuleType);
            var moduleCoreAttribute = moduleType.GetCustomAttribute<ModuleCoreAttribute>();

            var controllerTypes = AppCore.GetBindedTypes<IModuleController<TModuleType>>();

            using (var db = this.CreateUnitOfWork())
            {
                var config = db.Module.Where(x => x.UniqueKey == moduleType.FullName).FirstOrDefault();
                if (config == null)
                {
                    config = new ModuleConfig() { UniqueKey = moduleType.FullName, DateChange = DateTime.Now };
                    db.Module.Add(config);
                    db.SaveChanges();
                }

                module.ID = config.IdModule;
                module._moduleCaption = moduleCoreAttribute.Caption;
                module._moduleUrlName = moduleCoreAttribute.DefaultUrlName;

                var configurationManipulator = new ModuleConfigurationManipulator<TModuleType>(module, CreateValuesProviderForModule(module));
                configurationManipulator.Start(AppCore);
                module._configurationManipulator = configurationManipulator;

                var cfg = configurationManipulator.GetUsable<ModuleConfiguration<TModuleType>>();

                if (!string.IsNullOrEmpty(cfg.UrlName)) module._moduleUrlName = cfg.UrlName;
                module.InitModule(controllerTypes);

                _modules.RemoveAll(x => x.Item1 == typeof(TModuleType));
                LoadModuleCallModuleStart(module);
                _modules.Add(new Tuple<Type, ModuleBase<ApplicationCore>>(typeof(TModuleType), module));

                this.RegisterEvent(
                     Journaling.EventType.Error,
                    $"Загрузка модуля '{moduleType.FullName}'",
                    $"Модуль загружен на основе типа '{module.GetType().FullName}' с Id={config.IdModule}."
                );
            }
        }

        internal List<ModuleCore> GetModulesInternal()
        {
            lock (_syncRoot)
            {
                var module = _modules.
                    Select(x => (ModuleCore)x.Item2).
                    ToList();

                return module;
            }
        }

        internal ModuleCore GetModuleInternal(string urlName)
        {
            lock (_syncRoot)
            {
                var module = _modules.
                    Select(x => x.Item2).
                    OfType<ModuleCore>().
                    Where(x => !string.IsNullOrEmpty(x._moduleUrlName) && x._moduleUrlName.Equals(urlName, StringComparison.InvariantCultureIgnoreCase)).
                    FirstOrDefault();

                return module;
            }
        }

        internal ModuleCore GetModuleInternal(int moduleID)
        {
            lock (_syncRoot)
            {
                var module = _modules.
                    Select(x => x.Item2).
                    OfType<ModuleCore>().
                    Where(x => x.IdModule == moduleID).
                    FirstOrDefault();

                return module;
            }
        }

        internal ConfigurationValuesProvider CreateValuesProviderForModule(ModuleCore module)
        {
            var configurationValuesProvider = new ConfigurationValuesProvider();
            using (var db = new CoreContext())
            {
                var config = db.Module.FirstOrDefault(x => x.IdModule == module.ID);
                if (config != null)
                {
                    configurationValuesProvider.Load(config.Configuration);
                }
            }
            return configurationValuesProvider;
        }

        internal ApplyConfigurationResult ApplyModuleConfiguration<TModule, TConfiguration>(TConfiguration configuration, ModuleConfigurationManipulator<TModule> moduleConfigurationManipulator, TModule module)
            where TModule : ModuleCore<TModule>
            where TConfiguration : ModuleConfiguration<TModule>, new()
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var context = AppCore.GetUserContextManager().GetCurrentUserContext();

            var permissionCheck = module.CheckPermission(context, ModuleCore.PermissionSaveConfiguration);
            if (permissionCheck == CheckPermissionResult.Denied) return ApplyConfigurationResult.PermissionDenied;

            var moduleType = typeof(TModule);
            var moduleCoreAttribute = moduleType.GetCustomAttribute<ModuleCoreAttribute>();

            var urlNameEncoded = System.Web.HttpUtility.UrlEncode(configuration.UrlName);
            configuration.UrlName = urlNameEncoded;

            using (var db = this.CreateUnitOfWork())
            using (var scope = db.CreateScope())
            {
                var moduleConfig = db.Module.FirstOrDefault(x => x.IdModule == module.ID);
                if (moduleConfig == null)
                {
                    moduleConfig = new ModuleConfig() { UniqueKey = typeof(TModule).FullName, DateChange = DateTime.Now, IdUserChange = 0 };
                    db.Module.AddOrUpdate(moduleConfig);
                }

                moduleConfig.Configuration = configuration._valuesProvider.Save();
                moduleConfig.DateChange = DateTime.Now;
                moduleConfig.IdUserChange = context.GetIdUser();

                db.SaveChanges();
                scope.Commit();

                module.ID = moduleConfig.IdModule;
                module._moduleUrlName = string.IsNullOrEmpty(configuration.UrlName) ? moduleCoreAttribute.DefaultUrlName : configuration.UrlName;
                moduleConfigurationManipulator._valuesProviderUsable.Load(moduleConfig.Configuration);
            }

            return ApplyConfigurationResult.Success;
        }

        #endregion
    }
}

