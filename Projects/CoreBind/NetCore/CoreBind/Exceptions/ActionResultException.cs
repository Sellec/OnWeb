using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OnWeb.Exceptions
{
    /// <summary>
    /// Представляет ошибки, возникающие во время выполнения приложения, с результатом для вывода.
    /// </summary>
    public class ActionResultException : Exception
    {
        public ActionResultException(ActionResult actionResult, string message, params object[] _args)
            : base(string.Format(message, _args))
        {
            this.ActionResult = actionResult;
        }

        /// <summary>
        /// Результат для вывода в выходной поток пользователю.
        /// </summary>
        public ActionResult ActionResult
        {
            get;
            private set;
        }
    }
}