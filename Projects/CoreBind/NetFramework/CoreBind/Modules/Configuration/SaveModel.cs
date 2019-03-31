using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.CoreBind.Modules.Configuration
{
    public class SaveModel
    {
        [Required]
        [MaxLength(200)]
        [Display(Name = "Название модуля для URL")]
        public string ModuleName { get; set; }
    }
}