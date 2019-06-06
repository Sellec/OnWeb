using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Modules
{
    using Core.DB;
    using Core.Modules;

    /// <summary>
    /// Базовый класс контроллера. Должен использоваться для создания контроллеров для модулей, основанных на <see cref="ModuleCore"/>. 
    /// Переопределяет часть стандартного функционала и предлагает другие методы вместо стандартных (см., например, <see cref="ModuleControllerBase.OnBeforeExecution(ActionExecutingContext)"/>).
    /// </summary>
    public abstract class ModuleControllerUser<TModule> : ModuleControllerBase, IModuleController<TModule>
        where TModule : ModuleCore<TModule>
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

#pragma warning disable CS0618
        /// <summary>
        /// </summary>
        protected sealed override void displayPrepare(object model)
        {
            base.displayPrepare(model);

            this.onDisplayModule(model);
            //this.onDisplayBase($template);

            ////foreach($this.mExtensions as $key => $var) $this.assignRef($key."Ex", $var);

            ViewData["Module"] = Module;
            ViewData["CurrentUserContext"] = AppCore.GetUserContextManager().GetCurrentUserContext();

            //////////////this.assignRef("Main", ApplicationCore.Instance);
            //////////////this.assignRef("UserManager", UserManager.Instance);

            //////////////this.assign("IsAuthorized", !AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest);
            //////////////this.assign("IsDeveloper", AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser() == 19 || AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser() == 20);
            //////////////this.assign("IsAdminPanel", ModulesManager.getModuleByNameBase("Admin").checkPermission(ModuleCore.ACCESSADMIN));
            //////////////this.assign("IsSuperuser", AppCore.GetUserContextManager().GetCurrentUserContext().IsSuperuser);
            //////////////this.assign("IsAdmin", this.Module.checkPermission(ModuleCore.ACCESSADMIN));

            //////////////this.assign("User", AppCore.GetUserContextManager().GetCurrentUserContext().getData());

            //////////////var themeActive = ThemeManager.getActive(false);
            //////////////this.assignRef("ThemeFolder", themeActive != null ? themeActive.FolderName : "");
        }

        /// <summary>
        /// Представляет раздел по-умолчанию.
        /// </summary>
        public abstract ActionResult Index();
    }
#pragma warning restore CS0618

}
