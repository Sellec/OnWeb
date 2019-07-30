using System;

namespace OnWeb.Core.Exceptions
{
    /// <summary>
    /// </summary>
    public class BehaviourException : HandledException
    {
        public BehaviourException(string message)
            : base(message)
        {
        }

        public BehaviourException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}