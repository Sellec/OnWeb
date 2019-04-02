using OnUtils;
using OnUtils.Architecture.AppCore;
using System;

namespace OnWeb
{
    using Core;
    using Core.DB;
    using Core.Journaling;
    using ExecutionResultJournalName = ExecutionResult<Core.DB.JournalName>;

    /// <summary>
    /// Методы расширений для <see cref="Manager"/>.
    /// </summary>
    public static class ManagerExtensions
    {
        /// <summary>
        /// Регистрирует новый журнал или обновляет старый на основе типа <typeparamref name="TApplicationComponent"/>.
        /// </summary>
        /// <param name="component">Компонент приложения (см. <see cref="IComponentSingleton{TAppCore}"/>) для которого регистрируется событие.</param>
        /// <param name="nameJournal">См. <see cref="JournalName.Name"/>.</param>
        /// <returns>Возвращает объект <see cref="ExecutionResultJournalName"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="nameJournal"/> представляет пустую строку или null.</exception>
        public static ExecutionResult RegisterJournal<TApplicationComponent>(this TApplicationComponent component, string nameJournal)
            where TApplicationComponent : class, IComponentSingleton<ApplicationCore>
        {
            return component.GetAppCore().Get<IManager>().RegisterJournalTyped<TApplicationComponent>(nameJournal);
        }

        /// <summary>
        /// Регистрирует новое событие в журнале на основе типа <typeparamref name="TApplicationComponent"/>.
        /// </summary>
        /// <param name="component">Компонент приложения (см. <see cref="IComponentSingleton{TAppCore}"/>) для которого регистрируется событие.</param>
        /// <param name="eventType">См. <see cref="Journal.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="Journal.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="Journal.EventInfoDetailed"/>.</param>
        /// <returns>Возвращает объект с результатом выполнения операции. Если во время добавления события в журнал возникла ошибка, она будет отражена в сообщении <see cref="ExecutionResult.Message"/>.</returns>
        public static ExecutionResult RegisterEvent<TApplicationComponent>(this TApplicationComponent component, EventType eventType, string eventInfo, string eventInfoDetailed = null)
            where TApplicationComponent : class, IComponentSingleton<ApplicationCore>
        {
            return component.GetAppCore().Get<IManager>().RegisterEvent<TApplicationComponent>(eventType, eventInfo, eventInfoDetailed, null, null);
        }

        /// <summary>
        /// Регистрирует новое событие в журнале на основе типа <typeparamref name="TApplicationComponent"/>.
        /// </summary>
        /// <param name="component">Компонент приложения (см. <see cref="IComponentSingleton{TAppCore}"/>) для которого регистрируется событие.</param>
        /// <param name="eventType">См. <see cref="Journal.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="Journal.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="Journal.EventInfoDetailed"/>.</param>
        /// <param name="exception">См. <see cref="Journal.ExceptionDetailed"/>.</param>
        /// <returns>Возвращает объект с результатом выполнения операции. Если во время добавления события в журнал возникла ошибка, она будет отражена в сообщении <see cref="ExecutionResult.Message"/>.</returns>
        public static ExecutionResult RegisterEvent<TApplicationComponent>(this TApplicationComponent component, EventType eventType, string eventInfo, string eventInfoDetailed = null, Exception exception = null)
            where TApplicationComponent : class, IComponentSingleton<ApplicationCore>
        {
            return component.GetAppCore().Get<IManager>().RegisterEvent<TApplicationComponent>(eventType, eventInfo, eventInfoDetailed, null, exception);
        }

        /// <summary>
        /// Регистрирует новое событие в журнале на основе типа <typeparamref name="TApplicationComponent"/>.
        /// </summary>
        /// <param name="component">Компонент приложения (см. <see cref="IComponentSingleton{TAppCore}"/>) для которого регистрируется событие.</param>
        /// <param name="eventType">См. <see cref="Journal.EventType"/>.</param>
        /// <param name="eventInfo">См. <see cref="Journal.EventInfo"/>.</param>
        /// <param name="eventInfoDetailed">См. <see cref="Journal.EventInfoDetailed"/>.</param>
        /// <param name="eventTime">См. <see cref="Journal.DateEvent"/>. Если передано значение null, то событие записывается на момент вызова метода.</param>
        /// <param name="exception">См. <see cref="Journal.ExceptionDetailed"/>.</param>
        /// <returns>Возвращает объект с результатом выполнения операции. Если во время добавления события в журнал возникла ошибка, она будет отражена в сообщении <see cref="ExecutionResult.Message"/>.</returns>
        public static ExecutionResult RegisterEvent<TApplicationComponent>(this TApplicationComponent component, EventType eventType, string eventInfo, string eventInfoDetailed = null, DateTime? eventTime = null, Exception exception = null)
            where TApplicationComponent : class, IComponentSingleton<ApplicationCore>
        {
            return component.GetAppCore().Get<IManager>().RegisterEvent<TApplicationComponent>(eventType, eventInfo, eventInfoDetailed, eventTime, exception);
        }
    }
}
