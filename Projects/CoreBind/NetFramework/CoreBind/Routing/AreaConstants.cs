using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.CoreBind.Routing
{
    /// <summary>
    /// Представляет предопределенные значения для зон движка.
    /// </summary>
    public static class AreaConstants
    {
        /// <summary>
        /// Обозначение пользовательской части, т.е. всего, что не относится к панели управления.
        /// </summary>
        public const string User = "user";

        /// <summary>
        /// Обозначение панели управления.
        /// </summary>
        public const string AdminPanel = "admin";
    }
}
