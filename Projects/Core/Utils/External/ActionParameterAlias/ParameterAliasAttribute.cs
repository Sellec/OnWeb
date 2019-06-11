using System;

namespace External.ActionParameterAlias
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

        public string ParameterName { get; }

        public string AliasName { get; }

    }
}
