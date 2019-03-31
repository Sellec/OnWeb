using OnUtils;
using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Linq;

namespace OnWeb.Core
{
    /// <summary>
    /// Менеджер, управляющий визуальными темами приложения. В настоящий момент в состоянии полу-deprecated.
    /// </summary>
    public class ThemeManager : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IUnitOfWorkAccessor<UnitOfWork<DB.Theme>>
    {
        private DateTime? _currentThemeSetTime = null;
        private DB.Theme _currentTheme = null;

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
        /// Возвращает данные о текущей активной теме приложения.
        /// </summary>
        /// <returns></returns>
        public ExecutionResult<DB.Theme> GetActiveTheme()
        {
            try
            {
                if (!_currentThemeSetTime.HasValue || (DateTime.Now - _currentThemeSetTime.Value).TotalSeconds >= 10)
                {
                    using (var db = this.CreateUnitOfWork())
                    {
                        _currentTheme = db.Repo1.Where(r => r.Active > 0).FirstOrDefault();
                        _currentThemeSetTime = DateTime.Now;
                    }
                }

                return new ExecutionResult<DB.Theme>(true, null, _currentTheme);
            }
            catch (Exception ex)
            {
                _currentThemeSetTime = null;
                _currentTheme = null;
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка получения данных об активной теме приложения.", null, null, ex);
                return new ExecutionResult<DB.Theme>(false, "Во время получения данных об активной теме приложения произошла ошибка.");
            }
        }
    }
}
