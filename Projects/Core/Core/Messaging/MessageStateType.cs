using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Варианты состояния сообщения.
    /// </summary>
    public enum MessageStateType : byte
    {
        /// <summary>
        /// Не обработано коннекторами. Такое сообщение считывается в <see cref="ServiceBase{TMessageType}.GetUnsentMessages"/> и обрабатывается коннекторами.
        /// </summary>
        NotProcessed = 0,

        /// <summary>
        /// Отправлено. Такое сообщение больше не обрабатывается, считается отправленным.
        /// </summary>
        Sent = 1,

        /// <summary>
        /// Ошибка отправки. Такое сообщение больше не обрабатывается, считается отправленным. Свойство <see cref="DB.MessageQueue.State"/> будет содержать суть ошибки.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Требуется повторная обработка в коннекторе такого же типа. Это подходит для сообщений, которым требуется проверка состояния отправки во внешнем сервисе.
        /// </summary>
        /// <seealso cref="IntermediateStateMessage{TMessageType}.State"/>
        RepeatWithControllerType = 4,
    }
}
