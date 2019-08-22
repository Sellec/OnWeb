using OnUtils;

namespace OnWeb.Core.Users
{
    /// <summary>
    /// Представляет результат выполнения операции проверки реквизитов или авторизации.
    /// </summary>
    public class ExecutionAuthResult : ExecutionResult
    {
        /// <summary>
        /// </summary>
        public ExecutionAuthResult(eAuthResult authResult, string message = null) : base(authResult == eAuthResult.Success, message)
        {
            AuthResult = authResult;
        }

        /// <summary>
        /// Дополнительная информация о результате выполнения.
        /// </summary>
        public eAuthResult AuthResult { get; }
    }

    /// <summary>
    /// Представляет результат выполнения операции проверки реквизитов или авторизации с данными, полученными в процессе выполнения.
    /// </summary>
    /// <typeparam name="TResult">Тип данных, полученных в процессе выполнения.</typeparam>
    public class ExecutionAuthResult<TResult> : ExecutionResult<TResult>
    {
        /// <summary>
        /// </summary>
        public ExecutionAuthResult(eAuthResult authResult, string message = null, TResult result = default(TResult)) : base(authResult == eAuthResult.Success, message, result)
        {
            AuthResult = authResult;
        }

        /// <summary>
        /// Дополнительная информация о результате выполнения.
        /// </summary>
        public eAuthResult AuthResult { get; }
    }
}
