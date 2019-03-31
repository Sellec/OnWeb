using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.Core.Exceptions
{
    /// <summary>
    /// Представляет ошибку инициализации модуля.
    /// </summary>
    public class ModuleInitException : Exception
    {
        internal ModuleInitException(Type moduleType, string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.ModuleType = moduleType;
        }

        /// <summary>
        /// Тип модуля, при инициализации которого возникла ошибка.
        /// </summary>
        public Type ModuleType { get; }
    }
}
