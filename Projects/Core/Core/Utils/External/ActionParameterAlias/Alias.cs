namespace System
{
    /// <summary>
    /// Позволяет задать альтернативное имя аргумента метода, которое будет восприниматься парсером из адресной строки или из данных формы.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// Позволяет задать альтернативное имя аргумента метода, которое будет восприниматься парсером из адресной строки или из данных формы.
        /// </summary>
        public AliasAttribute(string aliasName)
        {
            Name = aliasName;
        }

        /// <summary>
        /// Альтернативное имя аргумента метода.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

    }
}

