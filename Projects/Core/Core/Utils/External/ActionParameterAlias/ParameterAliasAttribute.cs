namespace System
{
    /// <summary>
    /// ��������� ������ �������������� ��� ��������� ������, ������� ����� �������������� �������� �� �������� ������ ��� �� ������ �����.
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
        /// ��� ��������� ������.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// �������������� ��� ��������� ������.
        /// </summary>
        public string AliasName { get; }

    }
}
