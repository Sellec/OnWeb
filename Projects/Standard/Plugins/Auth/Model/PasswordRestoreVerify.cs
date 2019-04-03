using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Plugins.Auth.Model
{
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