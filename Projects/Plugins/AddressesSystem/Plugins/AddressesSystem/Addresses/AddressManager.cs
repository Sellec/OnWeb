using OnUtils;
using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using OnUtils.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.Addresses
{
    using DB;
    using global::DaData;
    using ServiceMonitor;

    class AddressManager : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IManager, IUnitOfWorkAccessor<UnitOfWork<Address, AddressSearchHistory>>, IMonitoredService
    {
        public class AddressLevel
        {
            public AddressLevel(int level)
            {
                this.Level = level;
            }

            public string KodAddress { get; set; } = "";

            public Address Address { get; set; }

            public int Level { get; set; }

            public override string ToString()
            {
                return $"{Level} - {KodAddress} - {Address.ToString()}";
            }
        }

        private Guid _serviceID = StringsHelper.GenerateGuid(nameof(AddressManager));
        private Lazy<DaDataClient> _dadataClient = null;
        private ConcurrentDictionary<string, DateTime> _cachedEvents = new ConcurrentDictionary<string, DateTime>();

        public AddressManager()
        {
            _dadataClient = new Lazy<DaDataClient>(GetDadataClient);
        }

        #region CoreComponentBase
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }
        #endregion

        ExecutionResult<Address> IManager.SearchAddress(string address)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var DadataClient = GetDadataClient();

                    DaData.Entities.AddressData result = null;

                    if (result == null)
                    {
                        try
                        {
                            var found = db.Repo2.Where(x => x.NameAddressSearch.Equals(address, StringComparison.InvariantCultureIgnoreCase) && x.ServiceFound == "DaData").OrderByDescending(x => x.DateSearch).FirstOrDefault();
                            if (found != null)
                            {
                                if (found.IsSuccess) result = Newtonsoft.Json.JsonConvert.DeserializeObject<DaData.Entities.AddressData>(found.ServiceAnswer);
                                else
                                {
                                    var timeoutForNewSearch = TimeSpan.FromDays(5); //Время, в течение которого мы используем старый неудачный результат поиска, чтобы не тратить деньги ЛК DaData.
                                    if (DateTime.Now - found.DateSearch < timeoutForNewSearch) return new ExecutionResult<Address>(false, "Указанный адрес не найден.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this.RegisterEvent(Journaling.EventType.Error, "Ошибка во время поиска кешированного результата.", $"Адрес: {address}", null, ex);
                        }
                    }

                    if (result == null)
                    {
                        var results = DadataClient.Clean<DaData.Entities.AddressData>(new string[] { address });
                        if (results.IsSuccess)
                        {
                            if (results.Data != null && results.Data.Count > 0)
                            {
                                var resultPre = results.Data.First();

                                var history = new AddressSearchHistory()
                                {
                                    NameAddressSearch = address,
                                    DateSearch = DateTime.Now,
                                    IsSuccess = false,
                                    AddressType = AddressType.Country,
                                    ServiceFound = "DaData",
                                    ServiceAnswer = Newtonsoft.Json.JsonConvert.SerializeObject(resultPre)
                                };

                                var allowed = resultPre.qc == DaData.Constants.QC.QC_OK;//Разрешаем только точные совпадения.
                                if (!allowed && resultPre.qc == DaData.Constants.QC.QC_UNSURE)
                                {
                                    int fias_level;
                                    if (int.TryParse(resultPre.fias_level, out fias_level))
                                    {
                                        if (fias_level >= 7) allowed = true;  //Если точность совпадения - улица, тоже разрешаем.
                                        if (fias_level == 6 && string.IsNullOrEmpty(resultPre.city_kladr_id) && !string.IsNullOrEmpty(resultPre.settlement_kladr_id)) allowed = true;//Если точность совпадения - некрупный населенный пункт, тоже разрешаем.
                                    }
                                }

                                if (allowed)
                                {
                                    result = resultPre;

                                    history.IsSuccess = true;
                                    history.KodAddress = result.kladr_id;
                                    history.ServiceAnswer = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                                }

                                db.Repo2.Add(history);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (results.Code == 402)
                            {
                                _cachedEvents.GetOrAddWithExpiration("account_balance_empty", (k) =>
                                {
                                    this.RegisterServiceState(ServiceStatus.CannotRunBecouseOfErrors, "Пополните баланс на счете Дадаты - не работают стандартизация и подсказки.");
                                    return DateTime.Now;
                                }, TimeSpan.FromMinutes(5));
                                return new ExecutionResult<Address>(false, "Поиск недоступен - недостаточно средств на счете Дадаты.");
                            }
                            else
                            {
                                this.RegisterServiceState(ServiceStatus.CannotRunBecouseOfErrors, "Необработанная ошибка в ответе сервиса.", new Exception($"Код ответа {results.Code}, текст: {results.Detail}"));
                                return new ExecutionResult<Address>(false, "Необработанная ошибка в ответе сервиса.");
                            }
                        }
                    }

                    var prepareResult = PrepareAddressDataIntoDB(address, result, db);
                    return new ExecutionResult<Address>(prepareResult != null, null, prepareResult);
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка во время обработки результата поиска.", $"SearchAddress: {address}", null, ex);
                return new ExecutionResult<Address>(false, "Ошибка во время обработки результата поиска.");
            }
        }

        public DaDataClient GetDadataClient()
        {
            return new DaDataClient(AppCore.Config.Get("dadataApiKey", ""), AppCore.Config.Get("dadataSecretKey", ""));
        }

        private AddressType getAddressType(string region, string district, string city, string street, string building, string building2)
        {
            if (!string.IsNullOrEmpty(building)) return AddressType.Building;
            if (!string.IsNullOrEmpty(building2)) return AddressType.Building;
            if (!string.IsNullOrEmpty(street)) return AddressType.Street;
            if (!string.IsNullOrEmpty(city)) return AddressType.City;
            if (!string.IsNullOrEmpty(district)) return AddressType.District;
            if (!string.IsNullOrEmpty(region)) return AddressType.Region;

            return AddressType.Region;
        }

        ///// <summary>
        ///// На основе геолокации пытается найти адрес по IP-адресу ("ДА Я ТИБЯ ПО АЙПИ ВЫЧИСЛЮ!!!!") из текущего запроса.
        ///// </summary>
        ///// <returns>
        ///// Возвращает null, если метод вызван не в контексте запроса.
        ///// Возвращает null или <see cref="DB.Address"/>, если адрес был успешно определен.
        ///// Тип возвращаемого адреса может быть любого уровня, от дома до страны.
        ///// </returns>
        //public ExecutionResult<Address> GetAddressByIP()
        //{
        //    try
        //    {
        //        if (HttpContext.Current != null && HttpContext.Current.Request != null)
        //        {
        //            System.Net.IPAddress ip = null;
        //            if (System.Net.IPAddress.TryParse(HttpContext.Current.Request.UserHostAddress, out ip))
        //            {
        //                var address = GetAddressByIP(ip);
        //                return address;
        //            }
        //        }
        //    }
        //    catch (Exception ex) { Debug.WriteLine("GetAddressByIP: {0}", ex); }
        //    return null;
        //}

        ExecutionResult<Address> IManager.GetAddressByIP(System.Net.IPAddress address)
        {
            return GetAddressByQuery(address.ToString(), addressString =>
            {
                var geo_result = GetDadataClient().QueryGeolocation(new DaData.Entities.Suggestions.GeolocationQuery("") { ip = address.ToString() });
                return geo_result.IsSuccess ? geo_result.Data?.location : null;
            });
        }

        ExecutionResult<Address> IManager.GetAddressByKladr(string kodKladr)
        {
            return GetAddressByQuery(kodKladr, addressString =>
            {
                var queryAddress = addressString;
                var isBuilding = false; //5000003918500140037_1
                if (kodKladr.Contains("_") || kodKladr.Length > 17)
                {
                    isBuilding = true;
                    queryAddress = queryAddress.Split('_').FirstOrDefault().Truncate(0, 17);
                }

                var geo_result = GetDadataClient().QueryAddressByKladrID(queryAddress);
                if (geo_result.IsSuccess)
                {
                    var addressData = geo_result.Data.suggestions?.FirstOrDefault();
                    if (addressData?.data != null)
                    {
                        if (addressData.data.fias_level == "7" && isBuilding)
                        {

                        }
                    }

                    return addressData;
                }
                else
                {
                    return null;
                }
            });
        }

        private ExecutionResult<Address> GetAddressByQuery(string addressString, Func<string, DaData.Entities.Suggestions.Suggestion<DaData.Entities.AddressData>> suggectionProvider)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    DaData.Entities.AddressData result = null;

                    if (result == null)
                    {
                        try
                        {
                            var found = db.Repo2.Where(x => x.NameAddressSearch.Equals(addressString, StringComparison.InvariantCultureIgnoreCase) && x.ServiceFound == "DaData" && x.AddressType == AddressType.IP_Address).OrderByDescending(x => x.DateSearch).FirstOrDefault();
                            if (found != null)
                            {
                                if (found.IsSuccess) result = Newtonsoft.Json.JsonConvert.DeserializeObject<DaData.Entities.AddressData>(found.ServiceAnswer);
                                else
                                {
                                    var timeoutForNewSearch = TimeSpan.FromMinutes(5); //Время, в течение которого мы используем старый неудачный результат поиска, чтобы не тратить счетчик подсказок DaData.
                                    if (DateTime.Now - found.DateSearch < timeoutForNewSearch) return new ExecutionResult<Address>(false, "Указанный адрес не найден.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this.RegisterEvent(Journaling.EventType.Error, "Ошибка во время поиска кешированного результата.", $"Адрес: {addressString}", null, ex);
                        }
                    }

                    AddressSearchHistory history = null;

                    if (result == null)
                    {
                        history = new AddressSearchHistory()
                        {
                            NameAddressSearch = addressString,
                            DateSearch = DateTime.Now,
                            IsSuccess = false,
                            AddressType = AddressType.IP_Address,
                            ServiceFound = "DaData"
                        };

                        var suggestion_result = suggectionProvider(addressString);
                        if (suggestion_result != null)
                        {
                            result = suggestion_result.data;

                            Debug.WriteLineNoLog("GetAddressByQuery: {0}", addressString);
                            Debug.WriteLineNoLog(Newtonsoft.Json.JsonConvert.SerializeObject(result));

                            history.IsSuccess = true;
                            history.KodAddress = result.kladr_id;
                            history.ServiceFound = "DaData";
                            history.ServiceAnswer = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                        }

                        db.Repo2.Add(history);
                        db.SaveChanges();
                    }

                    var prepareResult = PrepareAddressDataIntoDB(addressString, result, db);
                    if (prepareResult != null && history != null)
                    {
                        history.AddressType = prepareResult.AddressType;
                        db.SaveChanges();
                        throw new NotImplementedException("Здесь надо проверить корректность заполнения AddressType в базе после повторного вызова SaveChanges.");
                    }

                    return new ExecutionResult<Address>(prepareResult != null, null, prepareResult);
                }
            }
            catch (NotImplementedException) { throw; }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка во время обработки результата поиска.", $"GetAddressByQuery: {addressString}", null, ex);
                return new ExecutionResult<Address>(false, "Ошибка во время обработки результата поиска.");
            }
        }

        public Func<string, Dictionary<string, string>, UnitOfWork<DB.Address, DB.AddressSearchHistory>, DB.Address> PrepareAddressDataIntoDBCallback = null;

        private List<Address> PrepareAddressDataIntoDBResult(DaData.Entities.AddressData result, UnitOfWork<DB.Address, DB.AddressSearchHistory> db)
        {
            if (result != null)
            {
                var addressFinal = new DB.Address();
                var addresses = new List<DB.Address>();
                var nameFull = new Dictionary<string, string>();

                var resultData = TypeHelper.ObjectToDictionary(result).ToDictionary(x => x.Key, x => x.Value != null ? x.Value.ToString() : "");

                // Заметки:
                // 1. Если не указан район (пустой area), то предполагаем, что это городской округ и используем город в качестве обозначения района.
                // 2. Если указан settlement, то для значения "мкр" считаем, что это часть адреса (street) и KodCity у такого адреса будет равен коду города. Другие значения считаются отдельными населенными пунктами и получают свой собственный KodCity.
                // 3. Дадата отдает данные без кода КЛАДР дома (иногда), поэтому для таких случаев генерим код КЛАДР на основе кода улицы и номера дома.
                var prefixList = new string[] { "region", "area", "city", "settlement", "street", "house" };
                var prefixListFiltered = prefixList.Where(x => resultData.ContainsKey(x + "_kladr_id") && !string.IsNullOrEmpty(resultData[x + "_kladr_id"])).ToArray();

                // Условности. Заметки, п. 3.
                if (resultData.ContainsKey("house") && (!resultData.ContainsKey("house_kladr_id") || string.IsNullOrEmpty(resultData["house_kladr_id"])))
                {
                    resultData["house_kladr_id"] = resultData["kladr_id"] + "_" + resultData["house"];
                }

                foreach (var prefix in prefixListFiltered)
                {
                    nameFull.Add(prefix, resultData.ContainsKey(prefix + "_with_type") && !string.IsNullOrEmpty(resultData[prefix + "_with_type"]) ?
                                        resultData[prefix + "_with_type"] :
                                       ((string.IsNullOrEmpty(resultData.GetValueOrDefault(prefix + "_type", "")) ? "" : resultData.GetValueOrDefault(prefix + "_type", "") + ".").Replace("..", ".") + " " + resultData[prefix]).Trim());

                    var isRegion = false;
                    var isDistrict = false;
                    var isCity = false;

                    addressFinal.KodAddress = resultData[prefix + "_kladr_id"];

                    // Запись основных частей адреса - регион, район, город, улица, дом.
                    if (prefix == "region")
                    {
                        addressFinal.KodRegion = resultData[prefix + "_kladr_id"];
                        isRegion = true;
                    }

                    if (prefix == "area")
                    {
                        addressFinal.KodDistrict = resultData[prefix + "_kladr_id"];
                        isDistrict = true;
                    }

                    if (prefix == "city")
                    {
                        addressFinal.KodCity = resultData[prefix + "_kladr_id"];
                        isCity = true;
                    }

                    if (prefix == "street")
                    {
                        addressFinal.KodStreet = resultData[prefix + "_kladr_id"];
                    }

                    if (prefix == "house")
                    {
                        addressFinal.KodBuildingCommon = resultData[prefix + "_kladr_id"];
                        addressFinal.KodBuilding = addressFinal.KodBuildingCommon;// $"{resultData[prefix + "_kladr_id"]}_{resultData["house_type"]}{resultData["house"]}";
                    }

                    // Условности. Если в качестве региона город, то это город федерального значения. Он отмечается как область, район и город.
                    if (prefix == "region" && resultData[prefix + "_type"] == "г")
                    {
                        isRegion = true;
                        isDistrict = true;
                        isCity = true;
                        addressFinal.KodRegion = resultData[prefix + "_kladr_id"];
                        addressFinal.KodDistrict = resultData[prefix + "_kladr_id"];
                        addressFinal.KodCity = resultData[prefix + "_kladr_id"];
                    }

                    // Условности. Заметки, п. 1. Если работаем с городом, но при этом район не определился, то в качестве района указываем город. 
                    if (prefix == "city" && string.IsNullOrEmpty(addressFinal.KodDistrict))
                    {
                        isDistrict = true;
                        addressFinal.KodDistrict = addressFinal.KodCity;
                    }

                    // Условности. Заметки, п. 2.
                    if (prefix == "settlement")
                    {
                        // Если это деревня или село, то у него будет указан settlement_kladr_id и мы можем записать адрес как отдельную единицу. Собственно, только в этом случае сюда и попадем. 
                        // Для микрорайонов settlement_kladr_id не передается, поэтому сюда попадания и не должно быть, но на всякий случай добавим условие и сделаем запись в лог.
                        if (resultData[prefix + "_type"] != "мкр")
                        {
                            isCity = true;
                            addressFinal.KodCity = resultData[prefix + "_kladr_id"];
                        }
                        else
                        {
                            addressFinal.KodStreet = resultData[prefix + "_kladr_id"];
                        }
                    }
                    if (prefix == "street" || prefix == "house")
                    {
                        // Проверяем, есть ли settlement без settlement_kladr_id. Если есть, то это, скорее всего, микрорайон (тоже проверяем), а для микрорайонов не указывается id, поэтому он не фигурирует у нас в базе как отдельный адрес.
                        // В таком случае микрорайон обозначаем как часть адреса - пристыковываем к улице или номеру дома, если нет улицы.
                        if (resultData.ContainsKey("settlement_type") && resultData["settlement_type"] == "мкр" && string.IsNullOrEmpty(resultData["settlement_kladr_id"]))
                        {
                            var namepart = resultData.ContainsKey("settlement_with_type") && !string.IsNullOrEmpty(resultData["settlement_with_type"]) ?
                                        resultData["settlement_with_type"] :
                                        ((string.IsNullOrEmpty(resultData.GetValueOrDefault("settlement_type", "")) ? "" : resultData.GetValueOrDefault("settlement_type", "") + ".").Replace("..", ".") + " " + resultData[prefix]).Trim();

                            var prefix2 = nameFull.ContainsKey("street") ? "street" : "house";
                            if (prefix2 == prefix) nameFull[prefix2] = string.Join(", ", new string[] { namepart, nameFull[prefix2] }.Where(x => !string.IsNullOrEmpty(x)));
                        }
                    }


                    // Работаем с определенными данными дальше.
                    var addressLevel = new DB.Address()
                    {
                        KodAddress = resultData[prefix + "_kladr_id"],
                        NameAddress = resultData[prefix],
                        NameAddressShort = resultData[prefix + "_type"],
                        NameAddressFull = string.Join(", ", nameFull.Values.GroupBy(x => x).Select(x => x.Key).Where(x => !string.IsNullOrEmpty(x))),
                        KodFias = resultData.ContainsKey(prefix + "_fias_id") ? (Guid?)new Guid(resultData[prefix + "_fias_id"]) : null,
                        DateChange = DateTime.Now,
                        KodRegion = addressFinal.KodRegion,
                        KodDistrict = addressFinal.KodDistrict,
                        KodCity = addressFinal.KodCity,
                        KodStreet = addressFinal.KodStreet,
                        KodBuildingCommon = addressFinal.KodBuildingCommon,
                        KodBuilding = addressFinal.KodBuilding,
                        AddressType = getAddressType(addressFinal.KodRegion, addressFinal.KodDistrict, addressFinal.KodCity, addressFinal.KodStreet, addressFinal.KodBuildingCommon, addressFinal.KodBuilding),
                        IsRegion = isRegion,
                        IsDistrict = isDistrict,
                        IsCity = isCity,
                    };

                    // Проверяем маркер города.
                    if (isCity && resultData.ContainsKey("capital_marker"))
                    {
                        if (resultData["capital_marker"] == "1" || resultData["capital_marker"] == "3" || resultData["capital_marker"] == "3") addressLevel.IsDistrictCenter = true;
                        if (resultData["capital_marker"] == "3" || resultData["capital_marker"] == "3") addressLevel.IsRegionCenter = true;
                    }
                    if (isCity && isDistrict && isRegion)
                    {
                        addressLevel.IsRegionCenter = true;
                        addressLevel.IsDistrictCenter = true;
                    }

                    //
                    addresses.Add(addressLevel);

                    // Если это дом, то сохраняем адрес дважды - первый раз просто с kladr_id, а второй - kladr_id плюс номер дома. В кладр-апи была проблема, что один kladr_api соответствовал нескольким домам.
                    if (prefix == "house")
                    {
                        var kodAddress = $"{resultData[prefix + "_kladr_id"]}_{resultData["house_type"]}{resultData["house"]}".ToLower();

                        addresses.Add(new Address()
                        {
                            KodAddress = kodAddress,
                            NameAddress = addressLevel.NameAddress,
                            NameAddressShort = addressLevel.NameAddressShort,
                            NameAddressFull = addressLevel.NameAddressFull,
                            KodFias = addressLevel.KodFias,
                            DateChange = addressLevel.DateChange,
                            KodRegion = addressLevel.KodRegion,
                            KodDistrict = addressLevel.KodDistrict,
                            KodCity = addressLevel.KodCity,
                            KodStreet = addressLevel.KodStreet,
                            KodBuildingCommon = addressLevel.KodBuildingCommon,
                            KodBuilding = kodAddress,
                            AddressType = addressLevel.AddressType,
                            IsRegion = addressLevel.IsRegion,
                            IsDistrict = addressLevel.IsDistrict,
                            IsCity = addressLevel.IsCity,
                            IsDistrictCenter = addressLevel.IsDistrictCenter,
                            IsRegionCenter = addressLevel.IsRegionCenter,
                        });
                    }
                }

                if (addresses.Count > 0)
                {
                    var addressesToChange = addresses.Last().ToEnumerable().Union(addresses.Where(x => x.AddressType == AddressType.Building));

                    decimal?[] geo = new decimal?[] { null, null };
                    decimal d = 0;
                    if (resultData.ContainsKey("geo_lat") && !string.IsNullOrEmpty(resultData["geo_lat"]))
                    {
                        if (decimal.TryParse(resultData["geo_lat"], out d)) addressesToChange.ForEach(x => x.CoordinateX = d);
                        else if (decimal.TryParse(resultData["geo_lat"].Replace(".", ","), out d)) addressesToChange.ForEach(x => x.CoordinateX = d);
                    }
                    if (resultData.ContainsKey("geo_lon") && !string.IsNullOrEmpty(resultData["geo_lon"]))
                    {
                        if (decimal.TryParse(resultData["geo_lon"], out d)) addressesToChange.ForEach(x => x.CoordinateY = d);
                        else if (decimal.TryParse(resultData["geo_lon"].Replace(".", ","), out d)) addressesToChange.ForEach(x => x.CoordinateY = d);
                    }

                    if (resultData.ContainsKey("okato") && !string.IsNullOrEmpty(resultData["okato"])) addressesToChange.ForEach(x => x.Okato = resultData["okato"]);
                    if (resultData.ContainsKey("postal_code") && !string.IsNullOrEmpty(resultData["postal_code"])) addressesToChange.ForEach(x => x.ZipCode = resultData["postal_code"]);

                    var addressesToBase = new Dictionary<string, DB.Address>();
                    addresses.ForEach(x =>
                    {
                        if (!addressesToBase.ContainsKey(x.KodAddress))
                            addressesToBase[x.KodAddress] = x;
                    });

                    return addressesToBase.Values.ToList();
                }
            }

            return null;
        }

        private Address PrepareAddressDataIntoDB(string address, DaData.Entities.AddressData result, UnitOfWork<Address, AddressSearchHistory> db)
        {
            var addresses = PrepareAddressDataIntoDBResult(result, db);

            if (addresses != null && addresses.Count > 0)
            {
                var addressesFiltered = addresses.GroupBy(x => x.KodAddress).Select(x => x.First()).ToList();
                db.Repo1.AddOrUpdate(y => y.KodAddress, addressesFiltered.ToArray());
                db.SaveChanges();

                // Возвращаем последний из найденных адресов. Для зданий это будет kladr_id с номером дома. Для остальных адресов - просто последняя часть адреса.
                return addresses.LastOrDefault();
            }
            else
            {
                return null;
            }
        }

        ExecutionResult<Dictionary<DaData.Entities.AddressData, Address>> IManager.PrepareAddressDataIntoDB(DaData.Entities.AddressData[] results)
        {
            try
            {
                using (var db = new UnitOfWork<Address, AddressSearchHistory>())
                {
                    var addresses = new List<Address>();
                    var addressesOutput = new Dictionary<DaData.Entities.AddressData, Address>();

                    foreach (var result in results)
                    {
                        var addressesResult = PrepareAddressDataIntoDBResult(result, db);
                        if (addressesResult != null && addressesResult.Count > 0)
                        {
                            addresses.AddRange(addressesResult);
                            addressesOutput.Add(result, addressesResult.Last());
                        }
                    }

                    var addressesFiltered = addresses.GroupBy(x => x.KodAddress).Select(x => x.First()).ToList();

                    if (addresses.Count > 0)
                    {
                        db.Repo1.AddOrUpdate(y => y.KodAddress, addressesFiltered.ToArray());
                    }

                    using (var scope = db.CreateScope())
                    {
                        db.SaveChanges();
                        scope.Commit();
                    }
                    return new ExecutionResult<Dictionary<DaData.Entities.AddressData, Address>>(true, null, addressesOutput);
                }
            }
            catch (Exception ex)
            {
                this.RegisterEvent(Journaling.EventType.Error, "Ошибка при сохранении внешних результатов поиска DaData", null, null, ex);
                return new ExecutionResult<Dictionary<DaData.Entities.AddressData, Address>>(false, "При сохранении результатов поиска произошла непредвиденная ошибка.");
            }
        }

        #region ServiceMonitor.IMonitoredService
        Guid IMonitoredService.ServiceID
        {
            get { return _serviceID; }
        }

        string IMonitoredService.ServiceName
        {
            get { return "Менеджер адресов КЛАДР/ФИАС"; }
        }

        string IMonitoredService.ServiceStatusDetailed
        {
            get { return string.Empty; }
        }

        bool IMonitoredService.IsSupportsCurrentStatusInfo
        {
            get { return false; }
        }

        ServiceStatus IMonitoredService.ServiceStatus
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}

namespace System
{
    using OnWeb.Core.DB;
    using System.Linq;

    /// <summary>
    /// Методы расширений для менеджера адресов.
    /// </summary>
    public static class AddressQueryExtensions
    {
        /// <summary>
        /// TODO проверить на актуальность.
        /// </summary>
        public static IQueryable<Address> IncludeAddressLevels(this IQueryable<Address> query)
        {
            return query;//.Include(x => x.City);
        }
    }
}

