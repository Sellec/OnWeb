using System;

namespace OnWeb.Core.Messaging
{
    /// <summary>
    /// Предоставляет коннектору информацию о сообщении.
    /// </summary>
    public class ConnectorMessage<TMessageType> where TMessageType : MessageBase
    {
        private string _state = null;

        internal ConnectorMessage(IntermediateStateMessage<TMessageType> intermediateMessage)
        {
            MessageBody = intermediateMessage.Message;
            HandledState = ConnectorMessageStateType.NotHandled;
            State = intermediateMessage.State;
        }

        /// <summary>
        /// Если коннектор установил значение, отличное от <see cref="ConnectorMessageStateType.NotHandled"/>, то дальнейшая обработка коннекторами прекращается.
        /// </summary>
        public ConnectorMessageStateType HandledState { get; set; }

        /// <summary>
        /// Сообщение для отправки.
        /// </summary>
        public TMessageType MessageBody { get; }

        /// <summary>
        /// Состояние сообщения на момент обработки коннектором.
        /// </summary>
        public MessageStateType StateType { get; }

        /// <summary>
        /// Дополнительное состояние сообщения. 
        /// Может использоваться в коннекторах вместе с <see cref="MessageStateType.RepeatWithControllerType"/> для отслеживания состояния отправки во внешних сервисах.
        /// После обработки сообщения коннектором значение свойства записывается для дальнейшего использования, если <see cref="HandledState"/> установлено в соответствующее состояние.
        /// </summary>
        public string State
        {
            get => _state;
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length > 200) throw new ArgumentOutOfRangeException("Длина состояния не может превышать 200 символов.");
                _state = value;
            }
        }

    }

}
