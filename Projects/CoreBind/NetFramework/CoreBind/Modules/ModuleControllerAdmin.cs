using OnUtils.Data;
using System;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.CoreBind.Modules
{
    using Core.DB;
    using Core.Modules;
    using CoreBind.Types;
    using Routing;
    using Core.Configuration;

    [ModuleController(ControllerTypeAdmin.TypeID)]
    public abstract class ModuleControllerAdmin<TModule, TContext, TConfigurationSaveModel> : ModuleControllerUser<TModule, TContext>
        where TModule : ModuleCore<TModule>
        where TContext : UnitOfWorkBase, new()
        where TConfigurationSaveModel : Configuration.SaveModel, new()
    {
        /// <summary>
        /// См. <see cref="ModuleControllerUser{TModule, TContext}.Index"/>.
        /// </summary>
        public override ActionResult Index()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вызывается, когда требуется создать и отобразить форму настройки модуля.
        /// </summary>
        /// <returns></returns>
        [ModuleAction("config")]
        public virtual ActionResult Configuration()
        {
            return View("AdminForModules/Design/ModuleEdit.cshtml", new Configuration.SaveModel() { ModuleName = Module.GetConfiguration<ModuleConfiguration<TModule>>().UrlName });
        }

        public ActionResult ConfigurationSave(TConfigurationSaveModel formData)
        {
            var answer = JsonAnswer();

            try
            {
                if (ModelState.IsValid)
                {
                    var cfg = ConfigurationSaveCustom(formData, out var outputMessage);
                    if (cfg == null) answer.FromFail($"Сохранение настроек не удалось. {outputMessage}".TrimEnd());
                    else
                    {
                        Module.GetConfigurationManipulator().ApplyConfiguration(cfg);
                        answer.FromSuccess($"{outputMessage}".Trim());
                    }
                }
            }
            catch (Exception ex) { answer.FromException(ex); }

            return ReturnJson(answer);
        }

        /// <summary>
        /// Вызывается во время сохранения настроек модуля в функции <see cref="ConfigurationSave(TConfigurationSaveModel)"/>, если данные из формы были переданы корректно (т.е. если <see cref="Controller.ModelState"/> не содержит информации об ошибках валидации модели).
        /// Возвращает объект настроек модуля, который будет применен к модулю.
        /// </summary>
        /// <param name="formData">Содержит модель данных, переданных из формы.</param>
        /// <param name="outputMessage">Может содержать выходное сообщение, которое необходимо добавить к ответу сервера.</param>
        /// <returns>Возвращает объект настроек модуля. Если возвращает null, то сохранение настроек модуля прерывается.</returns>
        protected virtual ModuleConfiguration<TModule> ConfigurationSaveCustom(TConfigurationSaveModel formData, out string outputMessage)
        {
            if (formData == null)
            {
                outputMessage = "Из формы не передавались данные.";
                return null;
            }

            var cfg = Module.GetConfigurationManipulator().GetEditable<ModuleConfiguration<TModule>>();
            cfg.UrlName = formData.ModuleName;
            outputMessage = "";
            return cfg;
        }

        protected sealed override ActionResult ErrorHandled(Exception exception)
        {
            return base.ErrorHandled(exception);
        }

        protected sealed override ViewResult View(string viewName, string masterName, object model)
        {
            int framesSkip = 0;
            var trace = new System.Diagnostics.StackTrace(framesSkip);
            var frames = trace.GetFrame(0);
            var doNotUseMasterName = trace.GetFrames().Any(x=> x.GetMethod().Name == nameof(Configuration));


            if (RequestAnswerType.GetAnswerType() == RequestAnswerType.Types.Visual && string.IsNullOrEmpty(masterName) && !doNotUseMasterName)
                masterName = $"{nameof(Plugins.Admin)}/Design/baseAdmin.cshtml";

            if (viewName == "errorHandled.cshtml")
                viewName = $"{nameof(Plugins.Admin)}/Design/errorHandled.cshtml";

            if (viewName == "error404NotFound.cshtml")
                viewName = $"{nameof(Plugins.Admin)}/Design/error404NotFound.cshtml";

            return base.View(viewName, masterName, model);
        }
    }

    public abstract class ModuleControllerAdmin<TModule, TContext> : ModuleControllerAdmin<TModule, TContext, Configuration.SaveModel>
        where TModule : ModuleCore<TModule>
        where TContext : UnitOfWorkBase, new()
    {
    }

    public abstract class ModuleControllerAdmin<TModule> : ModuleControllerAdmin<TModule, CoreContext>
        where TModule : ModuleCore<TModule>
    {
    }

}