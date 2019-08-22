using System.ComponentModel.DataAnnotations;

namespace OnWeb.Core.Users
{
#pragma warning disable CS1591 // todo внести комментарии.
    public enum eUserAuthorizeAllowed : int
    {
        [Display(Name = "Авторизация отключена")]
        Nothing = 0,

        [Display(Name = "Только через E-mail")]
        OnlyEmail = 1,

        [Display(Name = "Только номер телефона")]
        OnlyPhone = 2,

        [Display(Name = "Email либо номер телефона")]
        EmailAndPhone = 3,
    }
}
