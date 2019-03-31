using System.ComponentModel.DataAnnotations;

namespace OnWeb.Core.Types
{
#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Режим регистрации на сайте.
    /// </summary>
    public enum RegisterMode : int
    {
        [Display(Name = "Регистрация без подтверждения")]
        Immediately = 0,

        [Display(Name = "Подтверждение по почте/телефону")]
        SelfConfirmation = 1,

        [Display(Name = "Проверка администратором")]
        ManualCheck = 2,

    }

}
