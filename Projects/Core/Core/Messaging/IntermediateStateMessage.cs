using System;

namespace OnWeb.Core.Messaging
{
    class IntermediateStateMessage<TMessageType> where TMessageType : MessageBase
    {
        internal IntermediateStateMessage(TMessageType message, DB.MessageQueue messageSource)
        {
            Message = message;
            MessageSource = messageSource;
        }

        public DB.MessageQueue MessageSource
        {
            get;
        }

        public int IdQueue
        {
            get => MessageSource.IdQueue;
        }

        public TMessageType Message
        {
            get;
        }

        public MessageStateType StateType
        {
            get => MessageSource.StateType;
            set => MessageSource.StateType = value;
        }

        public string State
        {
            get => MessageSource.State;
            set => MessageSource.State = value;
        }

        public int? IdTypeConnector
        {
            get => MessageSource.IdTypeConnector;
            set => MessageSource.IdTypeConnector = value;
        }

        public DateTime DateChange
        {
            set => MessageSource.DateChange = value;
        }
    }
}
