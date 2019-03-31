namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Описывает контакт сообщения.
    /// </summary>
    public class Contact<TContactType>
    {
        /// <summary>
        /// </summary>
        public Contact()
        {
        }

        /// <summary>
        /// </summary>
        public Contact(string name, TContactType contactData)
        {
            Name = name;
            ContactData = contactData;
        }

        /// <summary>
        /// Читаемое название/описание/имя/фио контакта.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Непосредственные данные контакта, используемые при отправке/получении сообщения.
        /// </summary>
        public TContactType ContactData { get; set; }

        /// <summary>
        /// </summary>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name)) return ContactData?.ToString();
            else
            {
                var c = ContactData?.ToString();
                if (!string.IsNullOrEmpty(c)) return $"{Name} ({c})";
                else return Name;
            }
        }
    }
}
