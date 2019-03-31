namespace DaData.Entities
{
    class ResponseBase
    {
        /// <summary>
        /// Указывает, был ли запрос выполнен успешно.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Код ответа.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Детальное сообщение.
        /// </summary>
        public string Detail { get; set; }
    }
}
