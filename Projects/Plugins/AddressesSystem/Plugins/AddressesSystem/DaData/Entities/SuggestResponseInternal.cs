using System.Collections.Generic;

namespace DaData.Entities
{
    class SuggestResponseInternal : ResponseBase
    {
        /// <summary>
        /// Данные из сервиса, если запрос выполнен без ошибок.
        /// </summary>
        public CleanResponseInternalData Data { get; set; }
    }

    class SuggestResponseInternalError
    {
        public string family { get; set; }

        public string reason { get; set; }

        public string message { get; set; }
    }

    class SuggestResponseInternalData
    {
        public IList<StructureType> structure { get; set; }

        public IList<IList<IDadataEntity>> data { get; set; }
    }
}
