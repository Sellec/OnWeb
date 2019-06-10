using System;

namespace OnWeb.Core.Configuration
{
    using Journaling;

    /// <summary>
    /// Предоставляет информацию о сохраняемых настройках модуля в событие <see cref="Modules.ModuleCore{TSelfReference}.OnConfigurationApply(ConfigurationApplyEventArgs{TSelfReference})"/>.
    /// </summary>
    public class ConfigurationApplyEventArgs<TModule> : EventArgs
        where TModule : Modules.ModuleCore<TModule>
    {
        internal ConfigurationApplyEventArgs(ModuleConfiguration<TModule> configuration)
        {
            Configuration = configuration;
            IsSuccess = true;
        }

        /// <summary>
        /// Указывает, что сохранение настроек должно быть прервано. 
        /// </summary>
        /// <param name="idJournalData">Идентификатор записи в журнале с информацией об ошибке.</param>
        /// <seealso cref="JournalingManager.GetJournalData(int)"/>
        public void SetFailed(int idJournalData)
        {
            IsSuccess = false;
            IdJournalData = idJournalData;
        }

        /// <summary>
        /// Новые настройки
        /// </summary>
        public ModuleConfiguration<TModule> Configuration { get; }

        internal bool IsSuccess { get; set; }

        internal int IdJournalData { get; set; }
    }
}
