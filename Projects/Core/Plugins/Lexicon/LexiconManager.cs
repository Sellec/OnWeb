using OnUtils.Application;
using OnUtils.Application.Journaling;
using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Plugins.Lexicon
{
    using DB;

    /// <summary>
    /// Менеджер для работы со словарными формами.
    /// </summary>
    public class LexiconManager : CoreComponentBase<WebApplicationCore>, IComponentSingleton<WebApplicationCore>, IUnitOfWorkAccessor<UnitOfWork<WordCase>>
    {
        /// <summary>
        /// Структура для запроса числительной и падежной формы слова.
        /// </summary>
        public class Request
        {
            /// <summary>
            /// Слово в именительном падеже единственном числе (см. <see cref="eNumeralType.SingleType"/>).
            /// </summary>
            public string Word { get; set; }

            /// <summary>
            /// Результат выполнения запроса.
            /// </summary>
            public WordCase Result { get; internal set; }
        }

        private static Lazy<OnlineMorpher> _morpher = new Lazy<OnlineMorpher>();

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
            AppCore.Get<JournalingManager>().RegisterJournalTyped<LexiconManager>("Журнал лексического менеджера");
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion

        static LexiconManager()
        {
        }

        //private static string GetWordCase(string word, int count, Func<WordCase, string> wordCaseCallback, string wordDefault = null)
        //{
        //    var request = Get(word, count);
        //    if (request != null && request.Result != null)
        //    {
        //        var wordCase = wordCaseCallback(request.Result);
        //        if (!string.IsNullOrEmpty(wordCase)) return wordCase;
        //    }
        //    return wordDefault;
        //}

        ///// <summary>
        ///// Возвращает именительный падеж слова <paramref name="word"/> в количественном варианте <paramref name="count"/> (см. <see cref="Request.Count"/>).
        ///// </summary>
        ///// <param name="word">См. <see cref="Request.Word"/>.</param>
        ///// <param name="count">См. <see cref="Request.Count"/>.</param>
        ///// <param name="wordDefault">Возвращается в качестве результата выполнения функции в случае, когда подходящая форма слова не была найдена.</param>
        ///// <returns></returns>
        //public static string GetNominative(string word, int count, string wordDefault = null)
        //{
        //    return GetWordCase(word, count, wordCases => wordCases.Nominative, wordDefault);
        //}

        ///// <summary>
        ///// Возвращает родительный падеж слова <paramref name="word"/> в количественном варианте <paramref name="count"/> (см. <see cref="Request.Count"/>).
        ///// </summary>
        ///// <param name="word">См. <see cref="Request.Word"/>.</param>
        ///// <param name="count">См. <see cref="Request.Count"/>.</param>
        ///// <param name="wordDefault">Возвращается в качестве результата выполнения функции в случае, когда подходящая форма слова не была найдена.</param>
        ///// <returns></returns>
        //public static string GetGenitive(string word, int count, string wordDefault = null)
        //{
        //    return GetWordCase(word, count, wordCases => wordCases.Genitive, wordDefault);
        //}

        ///// <summary>
        ///// Возвращает дательный падеж слова <paramref name="word"/> в количественном варианте <paramref name="count"/> (см. <see cref="Request.Count"/>).
        ///// </summary>
        ///// <param name="word">См. <see cref="Request.Word"/>.</param>
        ///// <param name="count">См. <see cref="Request.Count"/>.</param>
        ///// <param name="wordDefault">Возвращается в качестве результата выполнения функции в случае, когда подходящая форма слова не была найдена.</param>
        ///// <returns></returns>
        //public static string GetDative(string word, int count, string wordDefault = null)
        //{
        //    return GetWordCase(word, count, wordCases => wordCases.Dative, wordDefault);
        //}

        ///// <summary>
        ///// Возвращает винительный падеж слова <paramref name="word"/> в количественном варианте <paramref name="count"/> (см. <see cref="Request.Count"/>).
        ///// </summary>
        ///// <param name="word">См. <see cref="Request.Word"/>.</param>
        ///// <param name="count">См. <see cref="Request.Count"/>.</param>
        ///// <param name="wordDefault">Возвращается в качестве результата выполнения функции в случае, когда подходящая форма слова не была найдена.</param>
        ///// <returns></returns>
        //public static string GetAccusative(string word, int count, string wordDefault = null)
        //{
        //    return GetWordCase(word, count, wordCases => wordCases.Accusative, wordDefault);
        //}

        ///// <summary>
        ///// Возвращает творительный падеж слова <paramref name="word"/> в количественном варианте <paramref name="count"/> (см. <see cref="Request.Count"/>).
        ///// </summary>
        ///// <param name="word">См. <see cref="Request.Word"/>.</param>
        ///// <param name="count">См. <see cref="Request.Count"/>.</param>
        ///// <param name="wordDefault">Возвращается в качестве результата выполнения функции в случае, когда подходящая форма слова не была найдена.</param>
        ///// <returns></returns>
        //public static string GetInstrumental(string word, int count, string wordDefault = null)
        //{
        //    return GetWordCase(word, count, wordCases => wordCases.Instrumental, wordDefault);
        //}

        ///// <summary>
        ///// Возвращает предложный падеж слова <paramref name="word"/> в количественном варианте <paramref name="count"/> (см. <see cref="Request.Count"/>).
        ///// </summary>
        ///// <param name="word">См. <see cref="Request.Word"/>.</param>
        ///// <param name="count">См. <see cref="Request.Count"/>.</param>
        ///// <param name="wordDefault">Возвращается в качестве результата выполнения функции в случае, когда подходящая форма слова не была найдена.</param>
        ///// <returns></returns>
        //public static string GetPrepositional(string word, int count, string wordDefault = null)
        //{
        //    return GetWordCase(word, count, wordCases => wordCases.Prepositional, wordDefault);
        //}

        /// <summary>
        /// Обрабатывает один запрос на получение формы слова <paramref name="word"/>.
        /// </summary>
        /// <param name="word">См. <see cref="Request.Word"/>.</param>
        /// <returns></returns>
        public Request Get(string word)
        {
            return Get(new Request() { Word = word }.ToEnumerable()).FirstOrDefault();
        }

        /// <summary>
        /// Обрабатывает список запросов на получение форм слов.
        /// </summary>
        /// <returns>Возвращает переданный список запросов, для успешно обработанных запросов заполняется свойство <see cref="Request.Result"/>.</returns>
        public IEnumerable<Request> Get(IEnumerable<Request> requestList)
        {
            try
            {
                // todo setError(null);

                using (var db = this.CreateUnitOfWork())
                {
                    IQueryable<WordCase> query = null;
                    foreach (var request in requestList)
                    {
                        var q = db.Repo1.Where(x => x.NominativeSingle == request.Word);
                        query = query != null ? query.Union(q) : q;
                    }

                    var results = query.ToList();

                    foreach(var res in results)
                    {
                        requestList.Where(x => x.Word == res.NominativeSingle).ForEach(x => x.Result = res);
                    }

                    var upsertFields = new List<UpsertField>();

                    foreach (var request in requestList)
                    {
                        if (request.Result == null || request.Result.IsNewSingle || request.Result.IsNewTwo || request.Result.IsNewMultiple)
                        {
                            var numeralTypesList = new List<eNumeralType>();
                            if (request.Result == null || request.Result.IsNewSingle) numeralTypesList.Add(eNumeralType.SingleType);
                            if (request.Result == null || request.Result.IsNewTwo) numeralTypesList.Add(eNumeralType.TwoThreeFour);
                            if (request.Result == null || request.Result.IsNewMultiple) numeralTypesList.Add(eNumeralType.Multiple);

                            var list = numeralTypesList.ToDictionary(x => x, x => _morpher.Value.GetNumeralResult(request.Word, x));

                            if (request.Result == null)
                                request.Result = new WordCase() {
                                    NominativeSingle = request.Word,
                                    IsNewSingle = true,
                                    IsNewTwo = true,
                                    IsNewMultiple = true
                                };

                            var canBeUpdated = false;
                            if (list.ContainsKey(eNumeralType.SingleType) && list[eNumeralType.SingleType] != null)
                            {
                                upsertFields.AddRange(
                                    new UpsertField(nameof(WordCase.GenitiveSingle)),
                                    new UpsertField(nameof(WordCase.DativeSingle)),
                                    new UpsertField(nameof(WordCase.AccusativeSingle)),
                                    new UpsertField(nameof(WordCase.InstrumentalSingle)),
                                    new UpsertField(nameof(WordCase.PrepositionalSingle)),
                                    new UpsertField(nameof(WordCase.IsNewSingle))
                                );

                                request.Result.GenitiveSingle = list[eNumeralType.SingleType].Р ?? "";
                                request.Result.DativeSingle = list[eNumeralType.SingleType].Д ?? "";
                                request.Result.AccusativeSingle = list[eNumeralType.SingleType].В ?? "";
                                request.Result.InstrumentalSingle = list[eNumeralType.SingleType].Т ?? "";
                                request.Result.PrepositionalSingle = list[eNumeralType.SingleType].П ?? "";
                                request.Result.IsNewSingle = false;

                                canBeUpdated = true;
                            }

                            if (list.ContainsKey(eNumeralType.TwoThreeFour) && list[eNumeralType.TwoThreeFour] != null)
                            {
                                upsertFields.AddRange(
                                    new UpsertField(nameof(WordCase.NominativeTwo)),
                                    new UpsertField(nameof(WordCase.GenitiveTwo)),
                                    new UpsertField(nameof(WordCase.DativeTwo)),
                                    new UpsertField(nameof(WordCase.AccusativeTwo)),
                                    new UpsertField(nameof(WordCase.InstrumentalTwo)),
                                    new UpsertField(nameof(WordCase.PrepositionalTwo)),
                                    new UpsertField(nameof(WordCase.IsNewTwo))
                                );

                                request.Result.NominativeTwo = list[eNumeralType.TwoThreeFour].И ?? "";
                                request.Result.GenitiveTwo = list[eNumeralType.TwoThreeFour].Р ?? "";
                                request.Result.DativeTwo = list[eNumeralType.TwoThreeFour].Д ?? "";
                                request.Result.AccusativeTwo = list[eNumeralType.TwoThreeFour].В ?? "";
                                request.Result.InstrumentalTwo = list[eNumeralType.TwoThreeFour].Т ?? "";
                                request.Result.PrepositionalTwo = list[eNumeralType.TwoThreeFour].П ?? "";
                                request.Result.IsNewTwo = false;

                                canBeUpdated = true;
                            }

                            if (list.ContainsKey(eNumeralType.Multiple) && list[eNumeralType.Multiple] != null)
                            {
                                upsertFields.AddRange(
                                    new UpsertField(nameof(WordCase.NominativeMultiple)),
                                    new UpsertField(nameof(WordCase.GenitiveMultiple)),
                                    new UpsertField(nameof(WordCase.DativeMultiple)),
                                    new UpsertField(nameof(WordCase.AccusativeMultiple)),
                                    new UpsertField(nameof(WordCase.InstrumentalMultiple)),
                                    new UpsertField(nameof(WordCase.PrepositionalMultiple)),
                                    new UpsertField(nameof(WordCase.IsNewMultiple))
                                );

                                request.Result.NominativeMultiple = list[eNumeralType.Multiple].И ?? "";
                                request.Result.GenitiveMultiple = list[eNumeralType.Multiple].Р ?? "";
                                request.Result.DativeMultiple = list[eNumeralType.Multiple].Д ?? "";
                                request.Result.AccusativeMultiple = list[eNumeralType.Multiple].В ?? "";
                                request.Result.InstrumentalMultiple = list[eNumeralType.Multiple].Т ?? "";
                                request.Result.PrepositionalMultiple = list[eNumeralType.Multiple].П ?? "";
                                request.Result.IsNewMultiple = false;

                                canBeUpdated = true;
                            }

                            if (canBeUpdated)
                            {
                                db.Repo1.InsertOrDuplicateUpdate(request.Result.ToEnumerable(), upsertFields.ToArray());
                            }
                        }
                    }

                    return requestList;
                }
            }
            catch (Exception ex)
            {
                // todo setError(ex.Message);
                this.RegisterEvent(
                    EventType.Error,
                    "Get: ошибка получения данных.",
                    null,
                    null,
                    ex
                );
            }

            return null;
        }

        /// <summary>
        /// Обрабатывает новые добавленные слова, получая для них словарные формы.
        /// </summary>
        public void PrepareNewWords()
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var queryWords = db.Repo1.Where(x => x.IsNewSingle || x.IsNewTwo || x.IsNewMultiple).ToList();
                    foreach (var word in queryWords)
                    {
                        try
                        {
                            var upsertFields = new List<UpsertField>();

                            var numeralTypesList = new List<eNumeralType>();
                            if (word.IsNewSingle) numeralTypesList.Add(eNumeralType.SingleType);
                            if (word.IsNewTwo) numeralTypesList.Add(eNumeralType.TwoThreeFour);
                            if (word.IsNewMultiple) numeralTypesList.Add(eNumeralType.Multiple);

                            var list = numeralTypesList.ToDictionary(x => x, x => _morpher.Value.GetNumeralResult(word.NominativeSingle, x));

                            var canBeUpdated = false;
                            if (list.ContainsKey(eNumeralType.SingleType) && list[eNumeralType.SingleType] != null)
                            {
                                upsertFields.AddRange(
                                    new UpsertField(nameof(WordCase.GenitiveSingle)),
                                    new UpsertField(nameof(WordCase.DativeSingle)),
                                    new UpsertField(nameof(WordCase.AccusativeSingle)),
                                    new UpsertField(nameof(WordCase.InstrumentalSingle)),
                                    new UpsertField(nameof(WordCase.PrepositionalSingle)),
                                    new UpsertField(nameof(WordCase.IsNewSingle))
                                );

                                word.GenitiveSingle = list[eNumeralType.SingleType].Р;
                                word.DativeSingle = list[eNumeralType.SingleType].Д;
                                word.AccusativeSingle = list[eNumeralType.SingleType].В;
                                word.InstrumentalSingle = list[eNumeralType.SingleType].Т;
                                word.PrepositionalSingle = list[eNumeralType.SingleType].П;
                                word.IsNewSingle = false;

                                canBeUpdated = true;
                            }

                            if (list.ContainsKey(eNumeralType.TwoThreeFour) && list[eNumeralType.TwoThreeFour] != null)
                            {
                                upsertFields.AddRange(
                                    new UpsertField(nameof(WordCase.NominativeTwo)),
                                    new UpsertField(nameof(WordCase.GenitiveTwo)),
                                    new UpsertField(nameof(WordCase.DativeTwo)),
                                    new UpsertField(nameof(WordCase.AccusativeTwo)),
                                    new UpsertField(nameof(WordCase.InstrumentalTwo)),
                                    new UpsertField(nameof(WordCase.PrepositionalTwo)),
                                    new UpsertField(nameof(WordCase.IsNewTwo))
                                );

                                word.NominativeTwo = list[eNumeralType.TwoThreeFour].И;
                                word.GenitiveTwo = list[eNumeralType.TwoThreeFour].Р;
                                word.DativeTwo = list[eNumeralType.TwoThreeFour].Д;
                                word.AccusativeTwo = list[eNumeralType.TwoThreeFour].В;
                                word.InstrumentalTwo = list[eNumeralType.TwoThreeFour].Т;
                                word.PrepositionalTwo = list[eNumeralType.TwoThreeFour].П;
                                word.IsNewTwo = false;

                                canBeUpdated = true;
                            }

                            if (list.ContainsKey(eNumeralType.Multiple) && list[eNumeralType.Multiple] != null)
                            {
                                upsertFields.AddRange(
                                    new UpsertField(nameof(WordCase.NominativeMultiple)),
                                    new UpsertField(nameof(WordCase.GenitiveMultiple)),
                                    new UpsertField(nameof(WordCase.DativeMultiple)),
                                    new UpsertField(nameof(WordCase.AccusativeMultiple)),
                                    new UpsertField(nameof(WordCase.InstrumentalMultiple)),
                                    new UpsertField(nameof(WordCase.IsNewMultiple))
                                );

                                word.NominativeMultiple = list[eNumeralType.Multiple].И;
                                word.GenitiveMultiple = list[eNumeralType.Multiple].Р;
                                word.DativeMultiple = list[eNumeralType.Multiple].Д;
                                word.AccusativeMultiple = list[eNumeralType.Multiple].В;
                                word.InstrumentalMultiple = list[eNumeralType.Multiple].Т;
                                word.PrepositionalMultiple = list[eNumeralType.Multiple].П;
                                word.IsNewMultiple = false;

                                canBeUpdated = true;
                            }

                            if (canBeUpdated)
                            {
                                db.Repo1.InsertOrDuplicateUpdate(word.ToEnumerable(), upsertFields.ToArray());
                            }
                        }
                        catch (Exception ex)
                        {
                            this.RegisterEvent(
                                        EventType.Error,
                                        $"Get: ошибка получения данных для слова '{word.NominativeSingle}'.",
                                        null,
                                        null,
                                        ex
                            );
                        }
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // todo setError(ex.Message);
                this.RegisterEvent(
                            EventType.Error,
                            "Get: ошибка получения данных.",
                            null,
                            null,
                            ex
                        );
            }
        }


    }
}

