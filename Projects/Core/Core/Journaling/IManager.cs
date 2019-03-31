using OnUtils;
using OnUtils.Architecture.AppCore;
using System;

namespace OnWeb.Core.Journaling
{
    using ExecutionResultJournalName = ExecutionResult<DB.JournalName>;

    /// <summary>
    /// Представляет менеджер системных журналов. Позволяет создавать журналы, как привязанные к определенным типам, так и вручную, и регистрировать в них события.
    /// </summary>
    public interface IManager : IComponentSingleton<ApplicationCore>
    {
        #region Регистрация журналов
        /// <summary>
        /// Регистрирует новый журнал или обновляет старый по ключу <paramref name="uniqueKey"/> (если передан).
        /// </summary>
        /// <param name="idType">См. <see cref="DB.JournalName.IdJournalType"/>.</param>
        /// <param name="name">См. <see cref="DB.JournalName.Name"/>.</param>
        /// <param name="uniqueKey">См. <see cref="DB.JournalName.UniqueKey"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="name"/> представляет пустую строку или null.</exception>
        ExecutionResultJournalName RegisterJournal(int idType, string name, string uniqueKey = null);

        /// <summary>
        /// Регистрирует новый журнал или обновляет старый на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <param name="name">См. <see cref="DB.JournalName.Name"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="name"/> представляет пустую строку или null.</exception>
        ExecutionResultJournalName RegisterJournalTyped<TJournalTyped>(string name);
        #endregion

        #region Получить журналы
        /// <summary>
        /// Возвращает журнал по уникальному ключу <paramref name="uniqueKey"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="uniqueKey"/> представляет пустую строку или null.</exception>
        ExecutionResultJournalName GetJournal(string uniqueKey);

        /// <summary>
        /// Возвращает журнал по идентификатору <paramref name="IdJournal"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        ExecutionResultJournalName GetJournal(int IdJournal);

        /// <summary>
        /// Возвращает журнал на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        ExecutionResultJournalName GetJournalTyped<TJournalTyped>();
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
        /// <returns>Возвращает объект <see cref="ExecutionResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        ExecutionResult RegisterEvent(int IdJournal, EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null);

        /// <summary>
        /// Регистрирует новое событие в журнале на основе типа <typeparamref name="TJournalTyped"/>.
        /// </summary>
        /// <param name="eventType">См. <see cref="DB.Journal.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="DB.Journal.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="DB.Journal.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="DB.Journal.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="DB.Journal.ExceptionDetailed"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResult"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        ExecutionResult RegisterEvent<TJournalTyped>(EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null);
        #endregion

    }
}
