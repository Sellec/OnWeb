using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;

namespace OnWeb.Plugins.CoreModule
{
    using Core.Modules;

    /// <summary>
    /// Интерфейс ядра системы для управления основными функциями.
    /// </summary>
    [ModuleCore("Ядро системы")]
    public sealed class CoreModule : ModuleCore<CoreModule>, ICritical
    {
        internal static readonly Guid PermissionConfigurationSave = "perm_configSave".GenerateGuid();

        /// <summary>
        /// </summary>
        protected override void InitModuleCustom()
        {
            RegisterPermission(PermissionConfigurationSave, "Изменение настроек сайта.", "");

            // Ищем старый вариант основной конфигурации в config с именем main. Если есть, то переносим значения в конфигурацию Core модуля и удаляем старый конфиг из базы.
            using (var db = new UnitOfWork<Model.config>())
            using (var scope = db.CreateScope())
            {
                //var oldMainConfig = db.Repo1.FirstOrDefault(r => r.name == "main");
                //if (oldMainConfig != null)
                //{
                //    var moduleConfig = db.Repo1.FirstOrDefault(x => x.name == ModulesManager.DBConfigPrefix + this.ID);
                //    if (moduleConfig == null)
                //    {
                //        moduleConfig = new Core.DB.config() { DateChange = DateTime.Now.Timestamp(), IdUserChange = 0, name = ModulesManager.DBConfigPrefix + this.ID, serialized = "" };
                //        db.Repo1.AddOrUpdate(moduleConfig);
                //    }

                //    moduleConfig.serialized = oldMainConfig.serialized;
                //    moduleConfig.DateChange = oldMainConfig.DateChange;
                //    moduleConfig.IdUserChange = oldMainConfig.IdUserChange;

                //    db.Repo1.Delete(oldMainConfig);
                //    db.SaveChanges();
                //}

                scope.Commit();
            }

            OnConfigurationApplied();
        }

        /// <summary>
        /// </summary>
        protected internal override void OnConfigurationApplied()
        {
            var hasSmsService = AppCore.Get<MessagingSMS.IService>() != null;
            if (!hasSmsService)
            {
                var cfg = GetConfigurationManipulator().GetEditable<Core.Configuration.CoreConfiguration>();
                switch (cfg.userAuthorizeAllowed)
                {
                    case Core.Users.eUserAuthorizeAllowed.EmailAndPhone:
                    case Core.Users.eUserAuthorizeAllowed.OnlyPhone:
                        cfg.userAuthorizeAllowed = Core.Users.eUserAuthorizeAllowed.OnlyEmail;
                        this.RegisterEvent(Core.Journaling.EventType.CriticalError, "Сервис рассылки СМС не найден - режим авторизации сброшен", "Не найден сервис рассылки СМС-сообщений. В связи с этим режим авторизации пользователей изменен на 'Только Email'.");
                        GetConfigurationManipulator().ApplyConfiguration(cfg);
                        break;
                }
            }
        }
    }
}
