namespace System
{
    /// <summary>
    /// ��������� ������ �������������� ��� ��������� ������, ������� ����� �������������� �������� �� �������� ������ ��� �� ������ �����.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// ��������� ������ �������������� ��� ��������� ������, ������� ����� �������������� �������� �� �������� ������ ��� �� ������ �����.
        /// </summary>
        public AliasAttribute(string aliasName)
        {
            Name = aliasName;
        }

        /// <summary>
        /// �������������� ��� ��������� ������.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

    }
}

