using System;
using System.Web.Mvc;

namespace OnWeb.Core.Modules
{
    using Core;
    using Core.Modules;

    /// <summary>
    /// Базовый класс контроллера. Должен использоваться для создания контроллеров для модулей, основанных на <see cref="ModuleCore{TSelfReference}"/>. 
    /// Переопределяет часть стандартного функционала и предлагает другие методы вместо стандартных (см., например, <see cref="ModuleControllerBase.OnBeforeExecution(ActionExecutingContext)"/>).
    /// </summary>
    public abstract class ModuleControllerUser<TModule> : ModuleControllerBase, IModuleController<TModule>
        where TModule : ModuleCore<TModule>
    {
        private class CoreComponentImpl : CoreComponentBase
        {
            protected override void OnStart()
            {
            }

            protected override void OnStop()
            {
            }
        }

        internal void InitController(TModule module)
        {
            Module = module;
        }

        /// <summary>
        /// Текущий модуль, к которому относится обрабатываемый контроллером запрос.
        /// </summary>
        public TModule Module
        {
            get;
            private set;
        }

        internal override IModuleCore ModuleBase
        {
            get => Module;
        }

#pragma warning disable CS0618
        /// <summary>
        /// </summary>
        protected sealed override void OnViewPrepare(object model)
        {
            base.OnViewPrepare(model);

            OnViewModule(model);

            ViewData["Module"] = Module;
            ViewData["CurrentUserContext"] = AppCore.GetUserContextManager().GetCurrentUserContext();

            ViewData["ControllerThreadId"] = System.Threading.Thread.CurrentThread.ManagedThreadId;
            ViewData["QueriesFromBeginRequest"] = WebUtils.QueryLogHelper.GetQueries().Count;
            ViewData["TimeViewSend"] = DateTime.Now;
        }

        /// <summary>
        /// Представляет раздел по-умолчанию.
        /// </summary>
        public abstract ActionResult Index();
    }
#pragma warning restore CS0618

}
