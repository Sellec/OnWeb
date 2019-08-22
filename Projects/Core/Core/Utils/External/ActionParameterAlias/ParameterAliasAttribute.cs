namespace System
{
    /// <summary>
    /// ѕозвол€ет задать альтернативное им€ аргумента метода, которое будет восприниматьс€ парсером из адресной строки или из данных формы.
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

        /// <summary>
        /// »м€ аргумента метода.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// јльтернативное им€ аргумента метода.
        /// </summary>
        public string AliasName { get; }

    }
}
