using OnUtils.Application;
using OnUtils.Application.Configuration;
using OnUtils.Application.Journaling;
using OnUtils.Application.Modules.CoreModule;
using OnUtils.Application.Users;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Languages
{
    using Core;

    /// <summary>
    /// Менеджер языков системы.
    /// </summary>
    public class Manager : CoreComponentBase, IComponentSingleton, IUnitOfWorkAccessor<DB.DataContext>
    {
        /// <summary>
        /// </summary>
        public Manager()
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("ru-ru");
        }

        private Language CreateDefaultLanguage()
        {
            return new Language() { IdLanguage = -1, NameLanguage = "Системный язык" };
        }

        private Language CreateFromDB(DB.Language source)
        {
            return new Language() { IdLanguage = source.IdLanguage, NameLanguage = source.NameLanguage };
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

        #region Методы
        /// <summary>
        /// Возвращает список языковых пакетов, зарегистрированных в системе.
        /// </summary>
        /// <returns>Возвращает список языковых пакетов или null в случае ошибки.</returns>
        public List<Language> GetLanguages()
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    return db.Language.ToList().Select(x => CreateFromDB(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка при получении списка языковых пакетов", null, null, ex);
                return null;
            }
        }

        /// <summary>
        /// Возвращает языковой пакет, активный в системе по-умолчанию.
        /// </summary>
        /// <returns>Возвращает объект типа <see cref="Language"/> или null в случае ошибки.</returns>
        public Language GetSystemLanguage()
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var id = AppCore.AppConfig.IdSystemLanguage;
                    var query = from Language in db.Language
                                where Language.IsDefault != 0 || Language.IdLanguage == id
                                orderby (Language.IdLanguage == id ? 1 : 0) descending
                                select Language;

                    var data = query.FirstOrDefault();

                    return data != null ? CreateFromDB(data) : CreateDefaultLanguage();
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка при получении активного системного языкового пакета", null, null, ex);
                return null;
            }
        }

        /// <summary>
        /// Устанавливает в качестве системного языковой пакет <paramref name="language"/>.
        /// </summary>
        /// <returns>Возвращает true, если языковой пакет был успешно указан в качестве активного, либо false в случае ошибки.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="language"/> равен null.</exception>
        public Types.ConfigurationSaveResult SetSystemLanguage(Language language)
        {
            if (language == null) throw new ArgumentNullException(nameof(language));

            try
            {
                var module = AppCore.GetModulesManager().GetModule<CoreModule<WebApplication>>();
                var cfg = module.GetConfigurationManipulator().GetEditable<CoreConfiguration<WebApplication>>();
                cfg.IdSystemLanguage = language.IdLanguage;

                switch (module.GetConfigurationManipulator().ApplyConfiguration(cfg).Item1)
                {
                    case ApplyConfigurationResult.PermissionDenied:
                        return Types.ConfigurationSaveResult.PermissionsDenied;

                    case ApplyConfigurationResult.Success:
                        return Types.ConfigurationSaveResult.Success;

                    default:
                        return Types.ConfigurationSaveResult.UnknownError;
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка при сохранении активного системного языкового пакета", null, null, ex);
                return Types.ConfigurationSaveResult.UnknownError;
            }
        }

        /// <summary>
        /// Возвращает языковой пакет, активный для пользователя, ассоциированного с текущим контекстом (см. <see cref="UserContextManager{TApplication}.GetCurrentUserContext"/>).
        /// </summary>
        /// <returns>Возвращает объект типа <see cref="Language"/> или null в случае ошибки.</returns>
        public Language GetUserLanguage()
        {
            return GetUserLanguage(AppCore.GetUserContextManager().GetCurrentUserContext());
        }

        /// <summary>
        /// Возвращает языковой пакет, активный для пользователя, ассоциированного с контекстом <paramref name="context"/>.
        /// </summary>
        /// <returns>Возвращает объект типа <see cref="Language"/> или null в случае ошибки.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="context"/> равен null.</exception>
        Language GetUserLanguage(IUserContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var id = AppCore.AppConfig.IdSystemLanguage;
                    var query = from Language in db.Language
                                where Language.IsDefault != 0 || Language.IdLanguage == id
                                orderby (Language.IdLanguage == id ? 1 : 0) descending
                                select Language;

                    var data = query.FirstOrDefault();

                    return data != null ? CreateFromDB(data) : CreateDefaultLanguage();
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(EventType.Error, "Ошибка при получении активного системного языкового пакета", null, null, ex);
                return null;
            }
        }

        /// <summary>
        /// Устанавливает в качестве активного для пользователя, ассоциированного с текущим контекстом (см. <see cref="UserContextManager{TApplication}.GetCurrentUserContext"/>), языковой пакет <paramref name="language"/>.
        /// </summary>
        /// <returns>Возвращает true, если языковой пакет был успешно указан в качестве активного, либо false в случае ошибки.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="language"/> равен null.</exception>
        public bool SetUserLanguage(Language language)
        {
            return SetUserLanguage(AppCore.GetUserContextManager().GetCurrentUserContext(), language);
        }

        /// <summary>
        /// Устанавливает в качестве активного для пользователя, ассоциированного с контекстом <paramref name="context"/>, языковой пакет <paramref name="language"/>.
        /// </summary>
        /// <returns>Возвращает true, если языковой пакет был успешно указан в качестве активного, либо false в случае ошибки.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="context"/> равен null.</exception>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="language"/> равен null.</exception>
        public bool SetUserLanguage(IUserContext context, Language language)
        {
            // todo доделать, когда появятся персональные настройки пользователя. 
            return false;
        }
        #endregion
    }
}
