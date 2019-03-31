﻿using OnUtils.Architecture.AppCore;
using System.Collections.Generic;

namespace OnWeb.Core.Messaging
{
    using Connectors;

    /// <summary>
    /// Представляет менеджер, управляющий обменом сообщениями - уведомления, электронная почта, смс и прочее.
    /// </summary>
    public interface IMessagingManager : IComponentSingleton<ApplicationCore>, IAutoStart
    {
        /// <summary>
        /// Возвращает сервис отправки и приёма электронной почты.
        /// </summary>
        Email.IService Email { get; }

        /// <summary>
        /// Возвращает список коннекторов, поддерживающих обмен сообщениями указанного типа <typeparamref name="TMessage"/>.
        /// </summary>
        IEnumerable<IConnectorBase<TMessage>> GetConnectorsByMessageType<TMessage>() where TMessage : MessageBase, new();

        /// <summary>
        /// Возвращает список сервисов-получателей критических сообщений.
        /// </summary>
        IEnumerable<ICriticalMessagesReceiver> GetCriticalMessagesReceivers();

        /// <summary>
        /// Пересоздает текущий используемый список коннекторов с учетом настроек коннекторов. Рекомендуется к использованию в случае изменения настроек коннекторов.
        /// </summary>
        void UpdateConnectorsFromSettings();
    }
}
