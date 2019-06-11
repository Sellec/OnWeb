using OnUtils;
using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace OnWeb.Core.Journaling
{
    using Items;
    using ExecutionRegisterResult = ExecutionResult<int?>;
    using ExecutionResultJournalData = ExecutionResult<DB.Journal>;
    using ExecutionResultJournalDataList = ExecutionResult<List<DB.Journal>>;
    using ExecutionResultJournalName = ExecutionResult<DB.JournalName>;

    /// <summary>
    /// Представляет менеджер системных журналов. Позволяет создавать журналы, как привязанные к определенным типам, так и вручную, и регистрировать в них события.
    /// </summary>
    public sealed class JournalingManager : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IUnitOfWorkAccessor<UnitOfWork<DB.Journal, DB.JournalName>>
    {
        //Список журналов, основанных на определенном типе объектов.
        private ConcurrentDictionary<Type, ExecutionResultJournalName> _typedJournalsList = new ConcurrentDictionary<Type, ExecutionResultJournalName>();

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected sealed override void OnStart()
        {
        }

        /// <summary>
        /// </summary>
        protected sealed override void OnStop()
        {
        }
        #endregion

        #region Регистрация журналов
        /// <summary>
        /// Регистрирует новый журнал или обновляет старый по ключу <paramref name="uniqueKey"/> (если передан).
        /// </summary>
        /// <param name="idType">См. <see cref="DB.JournalName.IdJournalType"/>.</param>
        /// <param name="name">См. <see cref="DB.JournalName.Name"/>.</param>
        /// <param name="uniqueKey">См. <see cref="DB.JournalName.UniqueKey"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="name"/> представляет пустую строку или null.</exception>
        [ApiIrreversible]
        public ExecutionResultJournalName RegisterJournal(int idType, string name, string uniqueKey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

                var data = new DB.JournalName()
                {
                    IdJournalType = idType,
                    Name = name,
                    UniqueKey = uniqueKey
                };

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.RequiresNew))
                {
                    db.Repo2.AddOrUpdate(x => x.UniqueKey, data);
                    db.SaveChanges();
                    scope.Commit();
                }

                return new ExecutionResultJournalName(true, null, data);
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.RegisterJournal)}: {ex.ToString()}");
                return new ExecutionResultJournalName(false, $"Возникла ошибка во время регистрации журнала с именем '{name}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        /// <summary>
        /// Регистрирует новый журнал или обновляет старый на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <param name="name">См. <see cref="DB.JournalName.Name"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="name"/> представляет пустую строку или null.</exception>
        [ApiIrreversible]
        public ExecutionResultJournalName RegisterJournalTyped<TJournalTyped>(string name)
        {
            return RegisterJournal(JournalingConstants.IdSystemJournalType, name, JournalingConstants.TypedJournalsPrefix + typeof(TJournalTyped).FullName);
        }
        #endregion

        #region Получить журналы
        /// <summary>
        /// Возвращает журнал по уникальному ключу <paramref name="uniqueKey"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="uniqueKey"/> представляет пустую строку или null.</exception>
        [ApiIrreversible]
        public ExecutionResultJournalName GetJournal(string uniqueKey)
        {
            try
            {
                if (string.IsNullOrEmpty(uniqueKey)) throw new ArgumentNullException(nameof(uniqueKey));

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                {
                    var data = db.Repo2.Where(x => x.UniqueKey == uniqueKey).FirstOrDefault();
                    return new ExecutionResultJournalName(data != null, data != null ? null : "Журнал с указанным уникальным ключом не найден.", data);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.GetJournal)}(string): {ex.ToString()}");
                return new ExecutionResultJournalName(false, $"Возникла ошибка во время получения журнала с уникальным именем '{uniqueKey}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        /// <summary>
        /// Возвращает журнал по идентификатору <paramref name="IdJournal"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        [ApiIrreversible]
        public ExecutionResultJournalName GetJournal(int IdJournal)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                {
                    var data = db.Repo2.Where(x => x.IdJournal == IdJournal).FirstOrDefault();
                    return new ExecutionResultJournalName(data != null, data != null ? null : "Журнал с указанным уникальным идентификатором не найден.", data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.GetJournal)}(int): {ex.ToString()}");
                return new ExecutionResultJournalName(false, $"Возникла ошибка во время получения журнала с идентификатором '{IdJournal}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        /// <summary>
        /// Возвращает журнал на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        [ApiIrreversible]
        public ExecutionResultJournalName GetJournalTyped<TJournalTyped>()
        {
            return _typedJournalsList.GetOrAddWithExpiration(
                typeof(TJournalTyped),
                (t) => GetJournal(JournalingConstants.TypedJournalsPrefix + typeof(TJournalTyped).FullName),
                TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Возвращает события, связанные с объектом <paramref name="relatedItem"/> во всех журналах.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalDataList"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="relatedItem"/> равен null.</exception>
        [ApiIrreversible]
        public ExecutionResultJournalDataList GetJournalForItem(ItemBase relatedItem)
        {
            if (relatedItem == null) throw new ArgumentNullException(nameof(relatedItem));
            var itemType = ItemTypeFactory.GetItemType(relatedItem.GetType());
            if (itemType == null) return new ExecutionResultJournalDataList(false, "Ошибка получения данных о типе объекта.");

            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                {
                    var query = db.Repo1.Where(x => x.IdRelatedItem == relatedItem.ID && x.IdRelatedItemType == itemType.IdItemType);
                    var data = query.ToList();

                    return new ExecutionResultJournalDataList(true, null, data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.GetJournalForItem)}: {ex.ToString()}");
                return new ExecutionResultJournalDataList(false, $"Возникла ошибка во время получения событий. Смотрите информацию в системном текстовом журнале.");
            }
        }

        /// <summary>
        /// Возвращает событие с идентификатором <paramref name="idJournalData"/>. Все методы регистрации событий в результате содержат идентификатор созданной записи.
        /// </summary>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionResultJournalData"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionResultJournalData.Result"/> содержит информацию о событии.
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        [ApiIrreversible]
        public ExecutionResultJournalData GetJournalData(int idJournalData)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(TransactionScopeOption.Suppress))
                {
                    var query = db.Repo1.Where(x => x.IdJournalData == idJournalData);
                    var data = query.FirstOrDefault();

                    return new ExecutionResultJournalData(true, null, data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.GetJournalForItem)}: {ex.ToString()}");
                return new ExecutionResultJournalData(false, $"Возникла ошибка во время получения события. Смотрите информацию в системном текстовом журнале.");
            }
        }

        #endregion

        #region Записать в журнал
        /// <summary>
        /// Регистрирует новое событие в журнале <paramref name="IdJournal"/>.
        /// </summary>
        /// <param name="IdJournal">См. <see cref="DB.Journal.IdJournal"/>.</param>
        /// <param name="eventType">См. <see cref="DB.Journal.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="DB.Journal.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="DB.Journal.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="DB.Journal.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="DB.Journal.ExceptionDetailed"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionRegisterResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionRegisterResult.Result"/> содержит идентификатор записи журнала (см. также <see cref="GetJournalData(int)"/>).
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        [ApiIrreversible]
        public ExecutionRegisterResult RegisterEvent(int IdJournal, EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
        {
            return RegisterEventInternal(IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception);
        }

        /// <summary>
        /// Регистрирует новое событие в журнале на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <param name="eventType">См. <see cref="DB.Journal.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="DB.Journal.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="DB.Journal.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="DB.Journal.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="DB.Journal.ExceptionDetailed"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionRegisterResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionRegisterResult.Result"/> содержит идентификатор записи журнала (см. также <see cref="GetJournalData(int)"/>).
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        [ApiIrreversible]
        public ExecutionRegisterResult RegisterEvent<TJournalTyped>(EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
        {
            try
            {
                var journalResult = GetJournalTyped<TJournalTyped>();
                return !journalResult.IsSuccess ?
                    new ExecutionRegisterResult(false, journalResult.Message) :
                    RegisterEventInternal(journalResult.Result.IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.RegisterEvent)}: {ex.ToString()}");
                return new ExecutionRegisterResult(false, $"Возникла ошибка во время регистрации события в типизированный журнал '{typeof(TJournalTyped).FullName}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        /// <summary>
        /// Регистрирует новое событие, связанное с объектом <paramref name="relatedItem"/>, в журнале <paramref name="IdJournal"/>.
        /// </summary>
        /// <param name="IdJournal">См. <see cref="DB.Journal.IdJournal"/>.</param>
        /// <param name="relatedItem">См. <see cref="DB.Journal.IdJournal"/>.</param>
        /// <param name="eventType">См. <see cref="DB.Journal.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="DB.Journal.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="DB.Journal.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="DB.Journal.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="DB.Journal.ExceptionDetailed"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionRegisterResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionRegisterResult.Result"/> содержит идентификатор записи журнала (см. также <see cref="GetJournalData(int)"/>).
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="relatedItem"/> равен null.</exception>
        [ApiIrreversible]
        public ExecutionRegisterResult RegisterEventForItem(int IdJournal, ItemBase relatedItem, EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
        {
            if (relatedItem == null) throw new ArgumentNullException(nameof(relatedItem));
            var itemType = ItemTypeFactory.GetItemType(relatedItem.GetType());
            if (itemType == null) return new ExecutionRegisterResult(false, "Ошибка получения данных о типе объекта.");

            return RegisterEventInternal(IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception, relatedItem.ID, itemType.IdItemType);
        }

        /// <summary>
        /// Регистрирует новое событие, связанное с объектом <paramref name="relatedItem"/>, в журнале на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <param name="relatedItem">См. <see cref="DB.Journal.IdJournal"/>.</param>
        /// <param name="eventType">См. <see cref="DB.Journal.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="DB.Journal.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="DB.Journal.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="DB.Journal.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="DB.Journal.ExceptionDetailed"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionRegisterResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionRegisterResult.Result"/> содержит идентификатор записи журнала (см. также <see cref="GetJournalData(int)"/>).
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="relatedItem"/> равен null.</exception>
        [ApiIrreversible]
        public ExecutionRegisterResult RegisterEventForItem<TJournalTyped>(ItemBase relatedItem, EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
        {
            if (relatedItem == null) throw new ArgumentNullException(nameof(relatedItem));
            var itemType = ItemTypeFactory.GetItemType(relatedItem.GetType());
            if (itemType == null) return new ExecutionRegisterResult(false, "Ошибка получения данных о типе объекта.");

            try
            {
                var journalResult = GetJournalTyped<TJournalTyped>();
                return !journalResult.IsSuccess ? 
                    new ExecutionRegisterResult(false, journalResult.Message) : 
                    RegisterEventInternal(journalResult.Result.IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception, relatedItem.ID, itemType.IdItemType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.RegisterEventForItem)}: {ex.ToString()}");
                return new ExecutionRegisterResult(false, $"Возникла ошибка во время регистрации события в типизированный журнал '{typeof(TJournalTyped).FullName}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        private ExecutionRegisterResult RegisterEventInternal(int IdJournal, EventType eventType, string eventInfo, string eventInfoDetailed, DateTime? eventTime, Exception exception, int? idRelatedItem = null, int? idRelatedItemType = null)
        {
            try
            {
                if (IdJournal <= 0) throw new ArgumentOutOfRangeException(nameof(IdJournal));
                if (string.IsNullOrEmpty(eventInfo)) throw new ArgumentNullException(nameof(eventInfo));

                var exceptionDetailed = exception != null ? (exception.GetMessageExtended() + "\r\n" + exception.ToString()) : null;
                if (!string.IsNullOrEmpty(exceptionDetailed))
                {
                    var pos = exceptionDetailed.IndexOf("System.Web.Mvc.ActionMethodDispatcher.Execute", StringComparison.InvariantCultureIgnoreCase);
                    if (pos >= 0) exceptionDetailed = exceptionDetailed.Substring(0, pos);
                }

                var data = new DB.Journal()
                {
                    IdJournal = IdJournal,
                    EventType = eventType,
                    EventInfo = eventInfo?.Truncate(0, 300),
                    EventInfoDetailed = eventInfoDetailed,
                    ExceptionDetailed = exceptionDetailed,
                    DateEvent = eventTime ?? DateTime.Now,
                };

                using (var db = this.CreateUnitOfWork())
                {
                    DB.JournalName journalForCritical = null;

                    using (var scope = db.CreateScope(TransactionScopeOption.RequiresNew))
                    {
                        db.Repo1.Add(data);
                        db.SaveChanges();

                        if (eventType == EventType.CriticalError)
                        {
                            journalForCritical = db.Repo2.Where(x => x.IdJournal == IdJournal).FirstOrDefault();
                        }
                        scope.Commit();
                    }

                    if (eventType == EventType.CriticalError)
                    {
                        var body = $"Дата события: {data.DateEvent.ToString("dd.MM.yyyy HH:mm:ss")}\r\n";
                        body += $"Сообщение: {data.EventInfo}\r\n";
                        if (!string.IsNullOrEmpty(data.EventInfoDetailed)) body += $"Подробная информация: {data.EventInfoDetailed}\r\n";
                        if (!string.IsNullOrEmpty(data.ExceptionDetailed)) body += $"Исключение: {data.ExceptionDetailed}\r\n";

                        AppCore.Get<Messaging.MessagingManager>().GetCriticalMessagesReceivers().ForEach(x => x.SendToAdmin(journalForCritical != null ? $"Критическая ошибка в журнале '{journalForCritical.Name}'" : "Критическая ошибка", body));
                    }

                }

                return new ExecutionRegisterResult(true, null, data.IdJournalData);
            }
            catch (HandledException ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.RegisterEvent)}: {ex.InnerException?.ToString()}");
                return new ExecutionRegisterResult(false, $"Возникла ошибка во время регистрации события в журнал №{IdJournal}. {ex.Message} Смотрите информацию в системном текстовом журнале.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(JournalingManager).FullName}.{nameof(JournalingManager.RegisterEvent)}: {ex.ToString()}");
                return new ExecutionRegisterResult(false, $"Возникла ошибка во время регистрации события в журнал №{IdJournal}. Смотрите информацию в системном текстовом журнале.");
            }
        }

        #endregion
    }
}

