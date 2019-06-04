using System.ComponentModel.DataAnnotations;

namespace OnWeb.CoreBind.Modules.Configuration
{
    /// <summary>
    /// Базовый класс модели, получаемой из формы настроек модуля.
    /// </summary>
    public class SaveModel
    {
        /// <summary>
        /// URL-доступное имя модуля.
        /// </summary>
        [MaxLength(200)]
        [Display(Name = "URL-доступное имя модуля")]
        public string UrlName { get; internal set; }
    }
}
