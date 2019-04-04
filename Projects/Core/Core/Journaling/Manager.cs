﻿using OnUtils;
using OnUtils.Architecture.AppCore;
using OnUtils.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.Journaling
{
    using Items;
    using ExecutionResultJournalForItem = ExecutionResult<List<DB.Journal>>;
    using ExecutionResultJournalName = ExecutionResult<DB.JournalName>;

    class Manager : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IManager, IUnitOfWorkAccessor<UnitOfWork<DB.Journal, DB.JournalName>>
    {
        //Список журналов, основанных на определенном типе объектов.
        private ConcurrentDictionary<Type, ExecutionResultJournalName> _typedJournalsList = new ConcurrentDictionary<Type, ExecutionResultJournalName>();

        #region CoreComponentBase
        protected sealed override void OnStart()
        {
        }

        protected sealed override void OnStop()
        {
        }
        #endregion

        #region Регистрация журналов
        ExecutionResultJournalName IManager.RegisterJournal(int idType, string name, string uniqueKey)
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
                {
                    db.Repo2.AddOrUpdate(x => x.UniqueKey, data);
                    db.SaveChanges();
                }

                return new ExecutionResultJournalName(true, null, data);
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(Manager).FullName}.{nameof(IManager.RegisterJournal)}: {ex.ToString()}");
                return new ExecutionResultJournalName(false, $"Возникла ошибка во время регистрации журнала с именем '{name}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        ExecutionResultJournalName IManager.RegisterJournalTyped<TJournalTyped>(string name)
        {
            return (this as IManager).RegisterJournal(JournalingConstants.IdSystemJournalType, name, JournalingConstants.TypedJournalsPrefix + typeof(TJournalTyped).FullName);
        }
        #endregion

        #region Получить журналы
        ExecutionResultJournalName IManager.GetJournal(string uniqueKey)
        {
            try
            {
                if (string.IsNullOrEmpty(uniqueKey)) throw new ArgumentNullException(nameof(uniqueKey));

                using (var db = this.CreateUnitOfWork())
                using (var scope = db.CreateScope(System.Transactions.TransactionScopeOption.Suppress))
                {
                    var data = db.Repo2.Where(x => x.UniqueKey == uniqueKey).FirstOrDefault();
                    return new ExecutionResultJournalName(data != null, data != null ? null : "Журнал с указанным уникальным ключом не найден.", data);
                }
            }
            catch (ArgumentNullException) { throw; }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(Manager).FullName}.{nameof(IManager.GetJournal)}(string): {ex.ToString()}");
                return new ExecutionResultJournalName(false, $"Возникла ошибка во время получения журнала с уникальным именем '{uniqueKey}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        ExecutionResultJournalName IManager.GetJournal(int IdJournal)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var data = db.Repo2.Where(x => x.IdJournal == IdJournal).FirstOrDefault();
                    return new ExecutionResultJournalName(data != null, data != null ? null : "Журнал с указанным уникальным идентификатором не найден.", data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(Manager).FullName}.{nameof(IManager.GetJournal)}(int): {ex.ToString()}");
                return new ExecutionResultJournalName(false, $"Возникла ошибка во время получения журнала с идентификатором '{IdJournal}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        ExecutionResultJournalName IManager.GetJournalTyped<TJournalTyped>()
        {
            return _typedJournalsList.GetOrAddWithExpiration(
                typeof(TJournalTyped),
                (t) => (this as IManager).GetJournal(JournalingConstants.TypedJournalsPrefix + typeof(TJournalTyped).FullName),
                TimeSpan.FromMinutes(5));
        }

        ExecutionResultJournalForItem IManager.GetJournalForItem(ItemBase relatedItem)
        {
            if (relatedItem == null) throw new ArgumentNullException(nameof(relatedItem));
            var itemType = Items.ItemTypeFactory.GetItemType(relatedItem.GetType());
            if (itemType == null) return new ExecutionResultJournalForItem(false, "Ошибка получения данных о типе объекта.");

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var query = db.Repo1.Where(x => x.IdRelatedItem == relatedItem.ID && x.IdRelatedItemType == itemType.IdItemType);
                    var data = query.ToList();

                    return new ExecutionResultJournalForItem(true, null, data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(Manager).FullName}.{nameof(IManager.GetJournalForItem)}: {ex.ToString()}");
                return new ExecutionResultJournalForItem(false, $"Возникла ошибка во время получения событий. Смотрите информацию в системном текстовом журнале.");
            }
        }
        #endregion

        #region Записать в журнал
        ExecutionResult IManager.RegisterEvent(int IdJournal, EventType eventType, string eventInfo, string eventInfoDetailed, DateTime? eventTime, Exception exception)
        {
            return RegisterEventInternal(IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception);
        }

        ExecutionResult IManager.RegisterEvent<TJournalTyped>(EventType eventType, string eventInfo, string eventInfoDetailed, DateTime? eventTime, Exception exception)
        {
            try
            {
                var journalResult = (this as IManager).GetJournalTyped<TJournalTyped>();
                return !journalResult.IsSuccess ? journalResult : RegisterEventInternal(journalResult.Result.IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(Manager).FullName}.{nameof(IManager.RegisterEvent)}: {ex.ToString()}");
                return new ExecutionResult(false, $"Возникла ошибка во время регистрации события в типизированный журнал '{typeof(TJournalTyped).FullName}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        ExecutionResult IManager.RegisterEventForItem(int IdJournal, ItemBase relatedItem, EventType eventType, string eventInfo, string eventInfoDetailed, DateTime? eventTime, Exception exception)
        {
            if (relatedItem == null) throw new ArgumentNullException(nameof(relatedItem));
            var itemType = Items.ItemTypeFactory.GetItemType(relatedItem.GetType());
            if (itemType == null) return new ExecutionResult(false, "Ошибка получения данных о типе объекта.");

            return RegisterEventInternal(IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception, relatedItem.ID, itemType.IdItemType);
        }

        ExecutionResult IManager.RegisterEventForItem<TJournalTyped>(ItemBase relatedItem, EventType eventType, string eventInfo, string eventInfoDetailed, DateTime? eventTime, Exception exception)
        {
            if (relatedItem == null) throw new ArgumentNullException(nameof(relatedItem));
            var itemType = Items.ItemTypeFactory.GetItemType(relatedItem.GetType());
            if (itemType == null) return new ExecutionResult(false, "Ошибка получения данных о типе объекта.");

            try
            {
                var journalResult = (this as IManager).GetJournalTyped<TJournalTyped>();
                return !journalResult.IsSuccess ? journalResult : RegisterEventInternal(journalResult.Result.IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception, relatedItem.ID, itemType.IdItemType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(Manager).FullName}.{nameof(IManager.RegisterEventForItem)}: {ex.ToString()}");
                return new ExecutionResult(false, $"Возникла ошибка во время регистрации события в типизированный журнал '{typeof(TJournalTyped).FullName}'. Смотрите информацию в системном текстовом журнале.");
            }
        }

        private ExecutionResult RegisterEventInternal(int IdJournal, EventType eventType, string eventInfo, string eventInfoDetailed, DateTime? eventTime, Exception exception, int? idRelatedItem = null, int? idRelatedItemType = null)
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
                    db.Repo1.Add(data);
                    db.SaveChanges();

                    if (eventType == EventType.CriticalError)
                    {
                        //С машин разработчика не нужна рассылка. Мало ли сколько косяков мы сами себе генерим во время разработки.
                        //if (!Debug.IsDeveloper)
                        {
                            var journal = db.Repo2.FirstOrDefault(x => x.IdJournal == IdJournal);
                            var body = $"Дата события: {data.DateEvent.ToString("dd.MM.yyyy HH:mm:ss")}\r\n";
                            body += $"Сообщение: {data.EventInfo}\r\n";
                            if (!string.IsNullOrEmpty(data.EventInfoDetailed)) body += $"Подробная информация: {data.EventInfoDetailed}\r\n";
                            if (!string.IsNullOrEmpty(data.ExceptionDetailed)) body += $"Исключение: {data.ExceptionDetailed}\r\n";

                            AppCore.Get<Messaging.IMessagingManager>().GetCriticalMessagesReceivers().ForEach(x => x.SendToAdmin(journal != null ? $"Критическая ошибка в журнале '{journal.Name}'" : "Критическая ошибка", body));
                        }
                    }
                }

                return new ExecutionResult(true);
            }
            catch (HandledException ex)
            {
                Debug.WriteLine($"{typeof(Manager).FullName}.{nameof(IManager.RegisterEvent)}: {ex.InnerException?.ToString()}");
                return new ExecutionResult(false, $"Возникла ошибка во время регистрации события в журнал №{IdJournal}. {ex.Message} Смотрите информацию в системном текстовом журнале.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{typeof(Manager).FullName}.{nameof(IManager.RegisterEvent)}: {ex.ToString()}");
                return new ExecutionResult(false, $"Возникла ошибка во время регистрации события в журнал №{IdJournal}. Смотрите информацию в системном текстовом журнале.");
            }
        }

        #endregion
    }
}

