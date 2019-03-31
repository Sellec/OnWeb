using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Routing
{
    /// <summary>
    /// Представляет аргумент для вызова метода <see cref="RegisterItem.action"/>.
    /// </summary>
    public class ActionArgument
    {
        /// <summary>
        /// Название аргумента (используется значение <see cref="System.Reflection.ParameterInfo.Name"/>) метода.
        /// </summary>
        public string ArgumentName { get; set; }

        /// <summary>
        /// Значение аргумента.
        /// </summary>
        public object ArgumentValue { get; set; }
    }
}
