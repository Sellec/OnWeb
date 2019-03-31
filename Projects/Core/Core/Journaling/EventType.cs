using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Core.Journaling
{
    /// <summary>
    /// Типы событий в журналах.
    /// </summary>
    public enum EventType : byte
    {
        /// <summary>
        /// Простое событие.
        /// </summary>
        [Display(Name = "Событие")]
        Info = 1,

        /// <summary>
        /// Событие с предупреждением о чем-то важном.
        /// </summary>
        [Display(Name = "Предупреждение")]
        Warning = 2,

        /// <summary>
        /// Событие с ошибкой.
        /// </summary>
        [Display(Name = "Ошибка")]
        Error = 3,

        /// <summary>
        /// Событие с критической ошибкой. В случае возникнования подобной ошибки система автоматически отсылает письмо с уведомлением администратору сайта.
        /// </summary>
        [Display(Name = "Критическая ошибка")]
        CriticalError = 4,
    }
}
