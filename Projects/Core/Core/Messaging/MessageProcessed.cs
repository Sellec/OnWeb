using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Хранит информацию о сообщении в очереди сообщений.
    /// </summary>
    public class MessageProcessed<TMessageType> where TMessageType : MessageBase
    {
        internal MessageProcessed(TMessageType message, DB.MessageQueue db)
        {
            this.Message = message;
            this.IdMessage = db.IdQueue;
            this.ExternalID = db.ExternalID;
        }

        /// <summary>
        /// Внутренний идентификатор сообщения.
        /// </summary>
        public int IdMessage { get; }

        /// <summary>
        /// Сообщение для отправки.
        /// </summary>
        public TMessageType Message { get; }

        /// <summary>
        /// Должно быть установлено в true, если коннектор обработал сообщение и следует прекратить дальнейшую обработку. В этом случае сообщение исключается из обработки другими коннекторами в текущем цикле менеджера. 
        /// В следующем цикле менеджер отправит данное сообщение на повторную обработку, если флаг <see cref="IsSuccess"/> не был установлен в true (см. <see cref="IsSuccess"/>).
        /// Если равно false, то изменения в <see cref="IsHandled"/>, <see cref="ExternalID"/>, <see cref="IsError"/>, <see cref="ErrorText"/> игнорируются.
        /// </summary>
        public bool IsHandled { get; set; } = false;

        /// <summary>
        /// Если установлено в true, то сообщение помечается как отправленное и больше не обрабатывается менеджером сообщений.
        /// </summary>
        public bool IsSuccess { get; set; } = false;

        /// <summary>
        /// Указывает на возникновение ошибки во время обработки сообщения. См. также <see cref="ErrorText"/>.
        /// </summary>
        public bool IsError { get; set; } = false;

        /// <summary>
        /// Текст ошибки, в случае, если <see cref="IsError"/> равен true.
        /// </summary>
        public string ErrorText { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор сообщения во внешней системе (необязателен). Если задан, то сохраняется в базе данных. 
        /// </summary>
        /// <example>
        /// Это может быть использовано, например, для отслеживания статуса отправки сообщения во внешней системе обработки сообщений. 
        /// Например, зарегистрировать смс для отправки через Amazon SNS, записать в это поле идентификатор сообщения в Amazon SNS, НЕ помечать флаг <see cref="IsSuccess"/> как true (оставить false), отметить <see cref="IsHandled"/> как true. 
        /// Таким образом, в следующем цикле менеджера появится возможность проверить статус отправки сообщения в Amazon SNS и выставить <see cref="IsSuccess"/> в true в случае успешной отправки, либо вернуть ошибку и очистить <see cref="ExternalID"/> для повторных попыток отправки.
        /// </example>
        public string ExternalID { get; set; } = string.Empty;
    }
}
