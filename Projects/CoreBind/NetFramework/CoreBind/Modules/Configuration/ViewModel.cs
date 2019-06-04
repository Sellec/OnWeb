using System.ComponentModel.DataAnnotations;

namespace OnWeb.CoreBind.Modules.Configuration
{
    ///// <summary>
    ///// Базовый класс модели представления, используемой на форме настройки модуля. Базовое представление AdminForModules/Design/baseModuleEdit.cshtml пользуется моделью этого типа, чтобы получить url-доступное имя.
    ///// </summary>
    //public class ViewModel
    //{
    //    /// <summary>
    //    /// URL-доступное имя модуля.
    //    /// </summary>
    //    [Display(Name = "URL-доступное имя модуля")]
    //    public string UrlName { get; set; }
    //}

    ///// <summary>
    ///// Модель представления, используемая непосредственно в целевом представлении (см. параметр 'viewName' в <see cref="ModuleControllerAdmin{TModule, TConfigurationViewModel, TConfigurationSaveModel}.ConfigurationViewFill(ViewModel{TConfigurationViewModel}, out string)"/>.
    ///// Предоставляет доступ к пользовательской модели представления.
    ///// </summary>
    ///// <typeparam name="TModuleViewModel">Тип пользовательской модели представления.</typeparam>
    //public class ViewModel<TModuleViewModel> : ViewModel
    //{
    //    /// <summary>
    //    /// Пользовательская модель представления, которую можно использовать в конкретном модуле.
    //    /// </summary>
    //    public TModuleViewModel ModuleModel { get; set; }
    //}
}
