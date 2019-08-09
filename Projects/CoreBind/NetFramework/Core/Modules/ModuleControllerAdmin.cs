using OnUtils.Application.Configuration;
using System;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Core.Modules
{
    using Core.Configuration;
    using CoreBind.Routing;
    using Journaling;
    using Types;

    [ModuleController(ControllerTypeAdmin.TypeID)]
    public abstract class ModuleControllerAdmin<TModule, TConfigurationViewModel, TConfigurationSaveModel> : ModuleControllerUser<TModule>
        where TModule : ModuleCore<TModule>
        where TConfigurationSaveModel : Configuration.SaveModel, new()
        where TConfigurationViewModel : TConfigurationSaveModel, new()
    {
        /// <summary>
        /// См. <see cref="ModuleControllerUser{TModule}.Index"/>.
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
        public ActionResult Configuration()
        {
            var viewModel = new TConfigurationViewModel();
            ConfigurationViewFill(viewModel, out string viewName);
            if (string.IsNullOrEmpty(viewName)) viewName = "AdminForModules/Design/ModuleEdit.cshtml";

            viewModel.UrlName = Module.GetConfiguration<ModuleConfiguration<TModule>>().UrlName;

            return View(viewName, viewModel);
        }

        /// <summary>
        /// Вызывается, когда необходимо заполнить пользовательскую модель представления.
        /// </summary>
        /// <param name="viewModelForFill">Пользовательская модель представления для модуля <typeparamref name="TModule"/>.</param>
        /// <param name="viewName">Возвращает имя представления для конкретного модуля. Для представления в любом случае принудительно задается базовое представление 'AdminForModules/Design/baseModuleEdit.cshtml'. Если возвращает пустое значение, то используется представление по-умолчанию.</param>
        protected virtual void ConfigurationViewFill(TConfigurationViewModel viewModelForFill, out string viewName)
        {
            viewName = "";
        }

        public ActionResult ConfigurationSave(TConfigurationSaveModel formData)
        {
            var answer = JsonAnswer();

            try
            {
                var urlNameNew = string.Empty;
                if (Request.Form.HasKey(nameof(formData.UrlName)))
                {
                    var urlName = Request.Form.GetValues(nameof(formData.UrlName))[0];
                    if (!string.IsNullOrEmpty(urlName)) urlNameNew = urlName;
                }

                if (!string.IsNullOrEmpty(urlNameNew))
                {
                    var sameUrlName = AppCore.
                        GetModulesManager().
                        GetModules().
                        Where(x => x.UrlName.Equals(urlNameNew, StringComparison.InvariantCultureIgnoreCase) && x.IdModule != Module.IdModule).
                        Select(x => $"'{x.Caption}' ({x.IdModule})").
                        ToList();

                    if (sameUrlName.Count > 0)
                    {
                        ModelState.AddModelError(nameof(formData.UrlName), $"Такое значение URL-доступного имени используется в следующих модулях: {string.Join(", ", sameUrlName)}");
                    }

                    if (int.TryParse(urlNameNew, out int moduleId) && moduleId.ToString() == urlNameNew)
                        ModelState.AddModelError(nameof(formData.UrlName), $"Нельзя использовать число в качестве URL-доступного имени, так как парсер будет пытаться сопоставить число с идентификатором модуля, а не с URL-доступным именем.");
                }

                if (ModelState.IsValid)
                {
                    var urlNameSource = Module.GetConfiguration<ModuleConfiguration<TModule>>().UrlName;

                    var cfg = ConfigurationSaveCustom(formData, out var outputMessage);
                    if (cfg == null) answer.FromFail($"Сохранение настроек не удалось. {outputMessage}".TrimEnd());
                    else
                    {
                        cfg.UrlName = urlNameNew;
                        if (!string.Equals(urlNameSource, cfg.UrlName, StringComparison.InvariantCultureIgnoreCase))
                            outputMessage = $"Обратите внимание, что после изменения URL-доступного имени модуль станет недоступен по старому адресу. {outputMessage}".Trim();

                        var result = Module.GetConfigurationManipulator().ApplyConfiguration(cfg);
                        switch (result.Item1)
                        {
                            case ApplyConfigurationResult.PermissionDenied:
                                answer.FromFail($"Недостаточно прав для сохранения настроек. {outputMessage}".Trim());
                                break;

                            case ApplyConfigurationResult.Failed:
                                var journalData = AppCore.Get<JournalingManager>().GetJournalData(result.Item2.Value);
                                answer.FromFail($"Возникла ошибка при сохранении настроек: {(journalData?.Message ?? "текст ошибки не найден")}. {outputMessage}".Trim());
                                break;

                            case ApplyConfigurationResult.Success:
                                answer.FromSuccess($"{outputMessage}".Trim());
                                break;

                        }
                    }
                }
            }
            catch (Exception ex) { answer.FromException(ex); }

            return ReturnJson(answer);
        }

        /// <summary>
        /// Вызывается во время сохранения настроек модуля в функции <see cref="ConfigurationSave(TConfigurationSaveModel)"/>, если данные из формы были переданы корректно (т.е. если <see cref="Controller.ModelState"/> не содержит информации об ошибках валидации модели).
        /// Возвращает объект настроек модуля, который будет использован при сохранении.
        /// </summary>
        /// <param name="formData">Содержит модель данных, переданных из формы.</param>
        /// <param name="outputMessage">Может содержать выходное сообщение, которое необходимо добавить к ответу сервера.</param>
        /// <returns>Возвращает объект настроек модуля. Если возвращает null, то сохранение настроек модуля прерывается.</returns>
        protected virtual ModuleConfiguration<TModule> ConfigurationSaveCustom(TConfigurationSaveModel formData, out string outputMessage)
        {
            outputMessage = "";
            return Module.GetConfigurationManipulator().GetEditable<ModuleConfiguration<TModule>>();
        }

        /// <summary>
        /// </summary>
        protected sealed override ActionResult ErrorHandled(Exception exception)
        {
            return base.ErrorHandled(exception);
        }

        /// <summary>
        /// </summary>
        protected sealed override ViewResult View(string viewName, string masterName, object model)
        {
            int framesSkip = 0;
            var trace = new System.Diagnostics.StackTrace(framesSkip);
            var frames = trace.GetFrame(0);
            var doNotUseMasterName = trace.GetFrames().Any(x=> x.GetMethod().Name == nameof(Configuration));

            if (model is TConfigurationViewModel)
                masterName = $"AdminForModules/Design/baseModuleEdit.cshtml";

            if (RequestAnswerType.GetAnswerType() == RequestAnswerType.Types.Visual && string.IsNullOrEmpty(masterName) && !doNotUseMasterName)
                masterName = $"{nameof(OnWeb.Modules.Admin)}/Design/baseAdmin.cshtml";

            if (viewName == "errorHandled.cshtml")
                viewName = $"{nameof(OnWeb.Modules.Admin)}/Design/errorHandled.cshtml";

            if (viewName == "error404NotFound.cshtml")
                viewName = $"{nameof(OnWeb.Modules.Admin)}/Design/error404NotFound.cshtml";

            return base.View(viewName, masterName, model);
        }
    }

    public abstract class ModuleControllerAdmin<TModule> : ModuleControllerAdmin<TModule, Configuration.SaveModel, Configuration.SaveModel>
        where TModule : ModuleCore<TModule>
    {
    }

}