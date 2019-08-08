using System;
using System.Net;

namespace OnWeb.Core.Exceptions
{
    /// <summary>
    /// Представляет ошибки, возникающие во время выполнения приложения, с кодом ошибки.
    /// </summary>
    public class ErrorCodeException : HandledException
    {
        /// <summary>
        /// Создает новый экземпляр с указанным кодом ошибки и сообщением.
        /// </summary>
        public ErrorCodeException(HttpStatusCode code, string message) : this(code, message, null)
        {
        }

        /// <summary>
        /// Создает новый экземпляр с указанным кодом ошибки, сообщением и вложенным исключением.
        /// </summary>
        public ErrorCodeException(HttpStatusCode code, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }

        /// <summary>
        /// Код ошибки.
        /// </summary>
        public HttpStatusCode Code
        {
            get;
            private set;
        }
    }
}