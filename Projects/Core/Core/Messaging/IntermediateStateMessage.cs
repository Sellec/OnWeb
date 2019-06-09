using System;

namespace OnWeb.Core.Messaging
{
    class IntermediateStateMessage<TMessageType> where TMessageType : MessageBase
    {
        internal IntermediateStateMessage(TMessageType message, DB.MessageQueue db)
        {
            Message = message;
            IdQueue = db.IdQueue;
            StateType = db.StateType;
            State = db.State;
        }

        public int IdQueue { get; }

        public TMessageType Message { get; }

        public MessageStateType StateType { get; set; }

        public string State { get; set; }

        public int? IdTypeConnector { get; set; }

        public DateTime DateChange { get; set; }
    }


}
