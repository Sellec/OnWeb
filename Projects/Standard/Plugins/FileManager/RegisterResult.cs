using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnWeb.Modules.FileManager
{
    /// <summary>
    /// Перечисление, возвращаемое функцией <see cref="FileManager.Register(out DB.File, string, string, string, DateTime?)"/>.
    /// </summary>
    public enum RegisterResult
    {
        /// <summary>
        /// </summary>
        Success = 0,

        /// <summary>
        /// </summary>
        Error = 1,

        /// <summary>
        /// Файл не был найден по указанному пути.
        /// </summary>
        NotFound = 2,
    }
}