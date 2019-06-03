using System.ComponentModel.DataAnnotations;

namespace OnWeb.CoreBind.Modules.Configuration
{
    public class SaveModel
    {
        [MaxLength(200)]
        [Display(Name = "URL-доступное имя модуля")]
        public string ModuleName { get; set; }
    }
}