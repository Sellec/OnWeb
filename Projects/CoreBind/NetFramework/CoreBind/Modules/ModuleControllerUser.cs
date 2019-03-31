using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Modules
{
    using Core;
    using Core.DB;
    using Core.Modules;

    /// <summary>
    /// Базовый класс контроллера. Должен использоваться для создания контроллеров для модулей, основанных на <see cref="ModuleCore"/>. 
    /// Переопределяет часть стандартного функционала и предлагает другие методы вместо стандартных (см., например, <see cref="ModuleControllerBase.OnBeforeExecution(ActionExecutingContext)"/>).
    /// </summary>
    public abstract class ModuleControllerUser<TModule, TContext> : ModuleControllerBase, IModuleController<TModule>
        where TModule : ModuleCore
        where TContext : UnitOfWorkBase, new()
    {
        private class CoreComponentImpl : CoreComponentBase<ApplicationCore>
        {
            protected override void OnStart()
            {
            }

            protected override void OnStop()
            {
            }
        }

        [ThreadStatic]
        private static WebCore.Types.RequestSpecificItem<TContext> _dbContext = null;

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

        internal override ModuleCore ModuleBase
        {
            get => Module;
        }

        /// <summary>
        /// Возвращает экземпляр контекста базы данных. 
        /// Возвращаемое значение зависит от контекста запроса. Если вызов осуществляется не в контексте запроса, то возвращает null.
        /// </summary>
        public TContext DB
        {
            get
            {
                if (_dbContext == null) _dbContext = new WebCore.Types.RequestSpecificItem<TContext>(() => _dbContext = null);
                return _dbContext.Value;
            }
        }

#pragma warning disable CS0618
        /// <summary>
        /// </summary>
        protected sealed override void displayPrepare(object model)
        {
            base.displayPrepare(model);

            this.onDisplayModule(model);
            //this.onDisplayBase($template);

            ////foreach($this.mExtensions as $key => $var) $this.assignRef($key."Ex", $var);

            ViewData["Module"] = this;

            //////////////this.assignRef("Main", ApplicationCore.Instance);
            //////////////this.assignRef("UserManager", UserManager.Instance);

            //////////////this.assign("IsAuthorized", UserManager.Instance.isAuthorized);
            //////////////this.assign("IsDeveloper", UserManager.Instance.getID() == 19 || UserManager.Instance.getID() == 20);
            //////////////this.assign("IsAdminPanel", ModulesManager.getModuleByNameBase("Admin").checkPermission(ModuleCore.ACCESSADMIN));
            //////////////this.assign("IsSuperuser", UserManager.Instance.isSuperuser);
            //////////////this.assign("IsAdmin", this.Module.checkPermission(ModuleCore.ACCESSADMIN));

            //////////////this.assign("User", UserManager.Instance.getData());

            //////////////var themeActive = ThemeManager.getActive(false);
            //////////////this.assignRef("ThemeFolder", themeActive != null ? themeActive.FolderName : "");
        }

    }
#pragma warning restore CS0618

    /// <summary>
    /// Базовый класс контроллера. Должен использоваться для создания контроллеров для модулей, основанных на <see cref="ModuleCore"/>. 
    /// Переопределяет часть стандартного функционала и предлагает другие методы вместо стандартных (см., например, <see cref="ModuleControllerBase.OnBeforeExecution(ActionExecutingContext)"/>).
    /// </summary>
    public abstract class ModuleControllerUser<TModule> : ModuleControllerUser<TModule, CoreContext>
        where TModule : ModuleCore
    {
    }

}
