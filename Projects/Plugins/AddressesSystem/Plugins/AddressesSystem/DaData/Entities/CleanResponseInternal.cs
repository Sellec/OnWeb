using System.Collections.Generic;

namespace DaData.Entities
{
    class CleanResponseInternal: ResponseBase
    {
        /// <summary>
        /// Данные из сервиса, если запрос выполнен без ошибок.
        /// </summary>
        public CleanResponseInternalData Data { get; set; }
    }

    class CleanResponseInternalError
    {
        public string detail { get; set; }
    }

    class CleanResponseInternalData
    {
        public IList<StructureType> structure { get; set; }

        public IList<IList<IDadataEntity>> data { get; set; }
    }
}
