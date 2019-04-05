using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DaData
{
    using Entities;

    /// <summary>
    /// Клиент для взаимодействия с сервисами ДаДаты.
    /// </summary>
    class DaDataClient
    {
        const string SUGGESTIONS_URL = "/suggest/";
        const string ADDRESS_RESOURCE = SUGGESTIONS_URL + "address";
        const string PARTY_RESOURCE = SUGGESTIONS_URL + "party";
        const string BANK_RESOURCE = SUGGESTIONS_URL + "bank";
        const string FIO_RESOURCE = SUGGESTIONS_URL + "fio";
        const string EMAIL_RESOURCE = SUGGESTIONS_URL + "email";
        const string GEOLOCATION_RESOURCE = "detectAddressByIp";
        const string FINDADDRESSBYCODE_RESOURCE = "findById/address";

        static Dictionary<Type, StructureType> TYPE_TO_STRUCTURE = new Dictionary<Type, StructureType>() {
            { typeof(AddressData),      StructureType.ADDRESS },
            { typeof(AsIsData),         StructureType.AS_IS },
            { typeof(BirthdateData),    StructureType.BIRTHDATE },
            { typeof(EmailData),        StructureType.EMAIL },
            { typeof(NameData),         StructureType.NAME },
            { typeof(PassportData),     StructureType.PASSPORT },
            { typeof(PhoneData),        StructureType.PHONE },
            { typeof(VehicleData),      StructureType.VEHICLE }
        };

        private string _token;
        private string _secret;
        private string _urlClean;
        private CustomCreationConverter<IDadataEntity> _converter;

        static DaDataClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
        }

        /// <summary>
        /// Создает объект клиента с указанными токеном и ключом.
        /// </summary>
        public DaDataClient(string token, string secret) : this(token, secret, "dadata.ru", "http")
        {
        }

        private DaDataClient(string token, string secret, string hostname, string protocol)
        {
            _token = token;
            _secret = secret;
            _urlClean = String.Format("{0}://{1}/api/v2/clean", protocol, hostname);
            _converter = new CleanResponseConverter();
        }

        #region Стандартизация
        /// <summary>
        /// Выполняет запрос к сервису стандантизации DaData.ru.
        /// </summary>
        private CleanResponseInternal CleanRequest(CleanRequest request)
        {
            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Token " + _token);
            if (_secret != null) client.DefaultRequestHeaders.Add("X-Secret", _secret);

            var serialized = JsonConvert.SerializeObject(request, new StringEnumConverter());
            var responseTask = client.PostAsync(_urlClean, new System.Net.Http.StringContent(serialized));
            responseTask.Wait();
            var answerTask = responseTask.Result.Content.ReadAsStringAsync();
            answerTask.Wait();
            if (responseTask.Result.IsSuccessStatusCode)
            {
                var answerObject = JsonConvert.DeserializeObject<CleanResponseInternalData>(answerTask.Result, _converter);
                return new CleanResponseInternal()
                {
                    IsSuccess = true,
                    Detail = null,
                    Code = (int)responseTask.Result.StatusCode,
                    Data = answerObject
                };
            }
            else
            {
                var answerObject = JsonConvert.DeserializeObject<CleanResponseInternalError>(answerTask.Result, _converter);
                return new CleanResponseInternal()
                {
                    IsSuccess = false,
                    Detail = answerObject.detail,
                    Code = (int)responseTask.Result.StatusCode,
                    Data = null
                };
            }
        }

        /// <summary>
        /// Выполняет запрос к сервису стандантизации DaData.ru с получением данных типа <typeparamref name="T"/>.
        /// </summary>
        /// <param name="inputs">Массив данных для обработки в сервисе.</param>
        /// <typeparam name="T">Целевой тип данных, который должен стандартизироваться сервисом (IDadataEntity — AddressData, PhoneData и т.д.).</typeparam>
        public Response<IList<T>> Clean<T>(IEnumerable<string> inputs) where T : IDadataEntity
        {
            // infer structure from target entity type
            var structure = new List<StructureType>(
                new StructureType[] { TYPE_TO_STRUCTURE[typeof(T)] }
            );
            // transform enity list to CleanRequest data structure
            var data = new List<List<string>>();
            foreach (string input in inputs)
            {
                data.Add(new List<string>(new string[] { input }));
            }
            var request = new CleanRequest(structure, data);
            // get response and transform it to list of entities
            var response = CleanRequest(request);
            if (!response.IsSuccess) return new Response<IList<T>>(response);
            var outputs = new List<T>();
            foreach (IList<IDadataEntity> row in response.Data.data)
            {
                outputs.Add((T)row[0]);
            }
            return new Response<IList<T>>(response) { Data = outputs };
        }
        #endregion

        #region Подсказки
        public Response<Entities.Suggestions.SuggestAddressResponse> QueryAddress(string address)
        {
            return QueryAddress(new Entities.Suggestions.AddressSuggestQuery(address));
        }

        public Response<Entities.Suggestions.SuggestAddressResponse> QueryAddress(Entities.Suggestions.AddressSuggestQuery query)
        {
            return Execute<Entities.Suggestions.SuggestAddressResponse>(ADDRESS_RESOURCE, true, query);
        }

        public Response<Entities.Suggestions.SuggestAddressResponse> QueryAddressByKladrID(string address)
        {
            return QueryAddressByKladrID(new Entities.Suggestions.AddressSuggestQuery(address));
        }

        public Response<Entities.Suggestions.SuggestAddressResponse> QueryAddressByKladrID(Entities.Suggestions.AddressSuggestQuery query)
        {
            return Execute<Entities.Suggestions.SuggestAddressResponse>(FINDADDRESSBYCODE_RESOURCE, true, query);
        }

        public Response<Entities.Suggestions.SuggestBankResponse> QueryBank(string bank)
        {
            return QueryBank(new Entities.Suggestions.BankSuggestQuery(bank));
        }

        public Response<Entities.Suggestions.SuggestBankResponse> QueryBank(Entities.Suggestions.BankSuggestQuery query)
        {
            return Execute<Entities.Suggestions.SuggestBankResponse>(BANK_RESOURCE, true, query);
        }

        public Response<Entities.Suggestions.SuggestEmailResponse> QueryEmail(string email)
        {
            var query = new Entities.Suggestions.SuggestQuery(email);
            return Execute<Entities.Suggestions.SuggestEmailResponse>(EMAIL_RESOURCE, true, query);
        }

        public Response<Entities.Suggestions.SuggestFioResponse> QueryFio(string fio)
        {
            return QueryFio(new Entities.Suggestions.FioSuggestQuery(fio));
        }

        public Response<Entities.Suggestions.SuggestFioResponse> QueryFio(Entities.Suggestions.FioSuggestQuery query)
        {
            return Execute<Entities.Suggestions.SuggestFioResponse>(FIO_RESOURCE, true, query);
        }

        public Response<Entities.Suggestions.SuggestPartyResponse> QueryParty(string party)
        {
            return QueryParty(new Entities.Suggestions.PartySuggestQuery(party));
        }

        public Response<Entities.Suggestions.SuggestPartyResponse> QueryParty(Entities.Suggestions.PartySuggestQuery query)
        {
            return Execute<Entities.Suggestions.SuggestPartyResponse>(PARTY_RESOURCE, true, query);
        }

        public Response<Entities.Suggestions.GeolocationResponse> QueryGeolocation(Entities.Suggestions.GeolocationQuery query)
        {
            return Execute<Entities.Suggestions.GeolocationResponse>(GEOLOCATION_RESOURCE, false, query);
        }

        private Response<T> Execute<T>(string resource, bool isPost, Entities.Suggestions.SuggestQuery query) where T : class, new()
        {
            //todo учесть isBool

            var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", "Token " + _token);

            var url = "https://suggestions.dadata.ru/suggestions/api/4_1/rs/" + resource;

            var fieldsDictionary = query.GetType().GetFields().ToDictionary(x => x.Name, x => x.GetValue(query)?.ToString());
            var propertiesDictionary = query.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(query)?.ToString());
            var pairsDictionary = fieldsDictionary.Union(propertiesDictionary).Where(x => !string.IsNullOrEmpty(x.Key) && !string.IsNullOrEmpty(x.Value)).ToList();
            if (pairsDictionary.Count > 0)
            {
                url += "?" + string.Join("&", pairsDictionary.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            }

            var responseTask = client.GetAsync(url);//,, new System.Net.Http.StringContent(serialized, System.Text.Encoding.UTF8, "application/json"));
            responseTask.Wait();
            var answerTask = responseTask.Result.Content.ReadAsStringAsync();
            answerTask.Wait();
            if (responseTask.Result.IsSuccessStatusCode)
            {
                var answerObject = JsonConvert.DeserializeObject<T>(answerTask.Result);//, _converter);
                return new Response<T>(new ResponseBase() { IsSuccess = true, Code = (int)responseTask.Result.StatusCode, Detail = null }) { Data = answerObject };
            }
            else
            {
                var answerObject = JsonConvert.DeserializeObject<SuggestResponseInternalError>(answerTask.Result);//, _converter);
                var message = answerObject.message;
                if (responseTask.Result.StatusCode == HttpStatusCode.Forbidden) message = "Сервис подсказок недоступен. Проверьте раздел справки https://dadata.ru/api/suggest/#why-403-name";
                return new Response<T>(new ResponseBase() { IsSuccess = false, Code = (int)responseTask.Result.StatusCode, Detail = message });
            }
        }
        #endregion

    }
}

