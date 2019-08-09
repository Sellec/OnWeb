using System.ComponentModel.DataAnnotations;

namespace OnWeb.Modules.Auth.Model
{
    [reCAPTCHA.Model]
    public class PasswordRestoreSave
    {
        [StringLength(32)]
        [Display(Name = "Код подтверждения")]
        public string Code { get; set; }

        [StringLength(64)]
        [Display(Name = "Пароль"), Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}