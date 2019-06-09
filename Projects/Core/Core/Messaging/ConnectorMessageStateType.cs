namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Варианты состояния сообщения <see cref="ConnectorMessage{TMessageType}"/> после обработки в коннекторе.
    /// </summary>
    public enum ConnectorMessageStateType
    {
        /// <summary>
        /// Не обработано. Сообщение будет отправлено в следующий коннектор или обработано в следующий раз, если других коннекторов нет.
        /// </summary>
        NotHandled,

        /// <summary>
        /// Отправлено. Такое сообщение больше не обрабатывается, считается отправленным.
        /// </summary>
        Sent,

        /// <summary>
        /// Ошибка отправки. 
        /// Такое сообщение больше не обрабатывается, считается отправленным. 
        /// Свойство <see cref="ConnectorMessage{TMessageType}.State"/> может использоваться для хранения ошибки.
        /// </summary>
        Error,

        /// <summary>
        /// Требуется повторная обработка в коннекторе такого же типа. Это подходит для сообщений, которым требуется проверка состояния отправки во внешнем сервисе.
        /// </summary>
        /// <seealso cref="ConnectorMessage{TMessageType}.State"/>
        RepeatWithControllerType,
    }
}
