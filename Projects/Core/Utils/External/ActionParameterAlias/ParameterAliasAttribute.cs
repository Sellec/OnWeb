using System;

namespace External.ActionParameterAlias
{
    /// <summary>
    /// Позволяет задать альтернативное имя аргумента метода, которое будет восприниматься парсером из адресной строки или из данных формы.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ParameterAliasAttribute : Attribute
    {

        /// <summary>
        /// </summary>
        public ParameterAliasAttribute(string parameterName, string aliasName)
        {
            ParameterName = parameterName;
            AliasName = aliasName;
        }

        public string ParameterName { get; }

        public string AliasName { get; }

    }
}
