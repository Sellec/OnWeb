using OnUtils.Application.Journaling;
using OnUtils.Application.Journaling.Model;
using System;
using System.Collections.Generic;
using OnUtils;

namespace OnWeb.Journaling
{
    using Core;
    using Core.Items;
    using ExecutionRegisterResult = ExecutionResult<int?>;
    using ExecutionResultJournalData = ExecutionResult<JournalData>;
    using ExecutionResultJournalDataList = ExecutionResult<List<JournalData>>;
    using ExecutionResultJournalName = ExecutionResult<JournalInfo>;
    using JournalingManagerApp = JournalingManager<WebApplication>;

    /// <summary>
    /// Представляет менеджер системных журналов. Позволяет создавать журналы, как привязанные к определенным типам, так и вручную, и регистрировать в них события.
    /// </summary>
    public sealed class JournalingManager : CoreComponentBase, IComponentSingleton
    {
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
        /// <param name="idType">См. <see cref="JournalInfo.IdJournalType"/>.</param>
        /// <param name="name">См. <see cref="JournalInfo.Name"/>.</param>
        /// <param name="uniqueKey">См. <see cref="JournalInfo.UniqueKey"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="name"/> представляет пустую строку или null.</exception>
        [ApiIrreversible]
        public ExecutionResultJournalName RegisterJournal(int idType, string name, string uniqueKey = null)
        {
            return AppCore.Get<JournalingManagerApp>().RegisterJournal(idType, name, uniqueKey);
        }

        /// <summary>
        /// Регистрирует новый журнал или обновляет старый на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <param name="name">См. <see cref="JournalInfo.Name"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="name"/> представляет пустую строку или null.</exception>
        [ApiIrreversible]
        public ExecutionResultJournalName RegisterJournalTyped<TJournalTyped>(string name)
        {
            return AppCore.Get<JournalingManagerApp>().RegisterJournalTyped<TJournalTyped>(name);
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
            return AppCore.Get<JournalingManagerApp>().GetJournal(uniqueKey);
        }

        /// <summary>
        /// Возвращает журнал по идентификатору <paramref name="IdJournal"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        [ApiIrreversible]
        public ExecutionResultJournalName GetJournal(int IdJournal)
        {
            return AppCore.Get<JournalingManagerApp>().GetJournal(IdJournal);
        }

        /// <summary>
        /// Возвращает журнал на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        [ApiIrreversible]
        public ExecutionResultJournalName GetJournalTyped<TJournalTyped>()
        {
            return AppCore.Get<JournalingManagerApp>().GetJournalTyped<TJournalTyped>();
        }

        /// <summary>
        /// Возвращает события, связанные с объектом <paramref name="relatedItem"/> во всех журналах.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalDataList"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="relatedItem"/> равен null.</exception>
        [ApiIrreversible]
        public ExecutionResultJournalDataList GetJournalForItem(ItemBase relatedItem)
        {
            return AppCore.Get<JournalingManagerApp>().GetJournalForItem(relatedItem);
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
            return AppCore.Get<JournalingManagerApp>().GetJournalData(idJournalData);
        }

        #endregion

        #region Записать в журнал
        /// <summary>
        /// Регистрирует новое событие в журнале <paramref name="IdJournal"/>.
        /// </summary>
        /// <param name="IdJournal">См. <see cref="JournalData.JournalInfo"/>.</param>
        /// <param name="eventType">См. <see cref="JournalData.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="JournalData.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="JournalData.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="JournalData.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="JournalData.ExceptionDetailed"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionRegisterResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionRegisterResult.Result"/> содержит идентификатор записи журнала (см. также <see cref="GetJournalData(int)"/>).
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        [ApiIrreversible]
        public ExecutionRegisterResult RegisterEvent(int IdJournal, EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
        {
            return AppCore.Get<JournalingManagerApp>().RegisterEvent(IdJournal, eventType, eventInfo, eventInfoDetailed, eventTime, exception);
        }

        /// <summary>
        /// Регистрирует новое событие в журнале на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <param name="eventType">См. <see cref="JournalData.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="JournalData.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="JournalData.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="JournalData.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="JournalData.ExceptionDetailed"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionRegisterResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionRegisterResult.Result"/> содержит идентификатор записи журнала (см. также <see cref="GetJournalData(int)"/>).
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        [ApiIrreversible]
        public ExecutionRegisterResult RegisterEvent<TJournalTyped>(EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
        {
            return AppCore.Get<JournalingManagerApp>().RegisterEvent< TJournalTyped>(eventType, eventInfo, eventInfoDetailed, eventTime, exception);
        }

        /// <summary>
        /// Регистрирует новое событие, связанное с объектом <paramref name="relatedItem"/>, в журнале <paramref name="IdJournal"/>.
        /// </summary>
        /// <param name="IdJournal">См. <see cref="JournalData.JournalInfo"/>.</param>
        /// <param name="relatedItem">См. <see cref="JournalData.IdRelatedItem"/> и <see cref="JournalData.IdRelatedItemType"/>.</param>
        /// <param name="eventType">См. <see cref="JournalData.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="JournalData.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="JournalData.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="JournalData.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="JournalData.ExceptionDetailed"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionRegisterResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionRegisterResult.Result"/> содержит идентификатор записи журнала (см. также <see cref="GetJournalData(int)"/>).
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="relatedItem"/> равен null.</exception>
        [ApiIrreversible]
        public ExecutionRegisterResult RegisterEventForItem(int IdJournal, ItemBase relatedItem, EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
        {
            return AppCore.Get<JournalingManagerApp>().RegisterEventForItem(IdJournal, relatedItem, eventType, eventInfo, eventInfoDetailed, eventTime, exception);
        }

        /// <summary>
        /// Регистрирует новое событие, связанное с объектом <paramref name="relatedItem"/>, в журнале на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <param name="relatedItem">См. <see cref="JournalData.IdRelatedItem"/> и <see cref="JournalData.IdRelatedItemType"/>.</param>
        /// <param name="eventType">См. <see cref="JournalData.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="JournalData.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="JournalData.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="JournalData.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="JournalData.ExceptionDetailed"/>.</param>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionRegisterResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае успеха свойство <see cref="ExecutionRegisterResult.Result"/> содержит идентификатор записи журнала (см. также <see cref="GetJournalData(int)"/>).
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// </returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="relatedItem"/> равен null.</exception>
        [ApiIrreversible]
        public ExecutionRegisterResult RegisterEventForItem<TJournalTyped>(ItemBase relatedItem, EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
        {
            return AppCore.Get<JournalingManagerApp>().RegisterEventForItem< TJournalTyped>(relatedItem, eventType, eventInfo, eventInfoDetailed, eventTime, exception);
        }
        #endregion
    }
}

