using System.Collections.Generic;

namespace OnWeb.Messaging
{
    using Core;
    using Core.DB;
    using Modules.MessagingEmail;

    /// <summary>
    /// Представляет менеджер, управляющий подписками и рассылками сообщений.
    /// </summary>
    public interface ISubscriptionsManager : IComponentSingleton
    {
        /// <summary>
        /// Возвращает список подписок в системе.
        /// </summary>
        /// <param name="isEnabled">Если параметр равен null, то возврашаются выключенные и включенные подписки, в противном случае - только включенные либо выключенные.</param>
        /// <param name="isSubscriptionAllowed">Указывает возможность самостоятельной подписи для подписок, которые следует вернуть. Если равен null, то статус не фильтруется.</param>
        /// <returns></returns>
        List<Subscription> getList(bool? isEnabled = null, bool? isSubscriptionAllowed = null);

        /// <summary>
        /// Создание нового листа рассылки.
        /// </summary>
        /// <param name="name">Название листа рассылки.</param>
        /// <param name="allowSubscribe">Указывает возможность самостоятельного подписывания пользователем.</param>
        /// <returns></returns>
        Subscription create(string name, bool allowSubscribe);

        /// <summary>
        /// Рассылка писем по рассылке номер <paramref name="IdSubscription"/> с темой <paramref name="subject"/> с текстом <paramref name="body"/>.
        /// </summary>
        /// <param name="IdSubscription">Номер рассылки</param>
        /// <param name="subject">Тема письма. Если не указана, то используется название листа рассылки.</param>
        /// <param name="body">Тело письма. Обязательный параметр, письмо не может быть пустым.</param>
        /// <param name="files">Список файлов, которые следует прикрепить к телу письма.</param>
        /// <param name="excludedAddresses">Почтовые адреса, которые следует исключить во время рассылания писем.</param>
        /// <param name="contentType">Указывает тип содержимого для рассылки.</param>
        /// <returns></returns>
        bool send(int IdSubscription, string subject, string body, ContentType contentType, List<int> files = null, ICollection<string> excludedAddresses = null);

        /// <summary>
        /// Добавление нового подписчика в указанный лист.
        /// </summary>
        /// <param name="IdSubscription">Номер листа рассылки.</param>
        /// <param name="email">Подписываемый почтовый адрес.</param>
        /// <returns></returns>
        bool subscribeEmail(int IdSubscription, string email);

        /// <summary>
        /// Добавление новой роли в указанный лист.
        /// </summary>
        /// <param name="IdSubscription">Номер листа рассылки.</param>
        /// <param name="IdRole">Номер роли.</param>
        /// <returns></returns>
        bool subscribeRole(int IdSubscription, int IdRole);

        /// <summary>
        /// Удаление указанного подписчика из рассылки.
        /// </summary>
        /// <param name="IdSubscription">Номер листа рассылки.</param>
        /// <param name="email">Почтовый адрес, который следует удалить.</param>
        /// <returns></returns>
        bool unsubscribeEmail(int IdSubscription, string email);

    }
}