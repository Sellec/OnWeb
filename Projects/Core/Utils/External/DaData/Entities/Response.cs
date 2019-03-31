namespace DaData.Entities
{
    class Response<T> where T : class
    {
        internal Response(ResponseBase responseBase)
        {
            this.IsSuccess = responseBase.IsSuccess;
            this.Code = responseBase.Code;
            this.Detail = responseBase.Detail;
        }

        /// <summary>
        /// Указывает, был ли запрос выполнен успешно.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Код ответа
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Детальное сообщение.
        /// </summary>
        public string Detail { get; }

        /// <summary>
        /// Возвращаемые данные в случае успеха.
        /// </summary>
        public T Data { get; internal set; }
    }

}
