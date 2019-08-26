using OnUtils.Application.Messaging.Components;
using OnUtils.Application.Messaging.Messages;
using OnUtils.Architecture.ObjectPool;
using System;

namespace OnWeb.Messaging.Components
{
    /// <summary>
    /// Базовый класс компонента сервиса обработки сообщений определенного типа.
    /// </summary>
    public abstract class MessageServiceComponent<TMessage> : Core.CoreComponentBase, IMessageServiceComponent<WebApplication, TMessage>
        where TMessage : MessageBase, new()
    {
        private readonly string _name;
        private readonly uint? _order;

        /// <summary>
        /// Создает новый экземпляр компонента.
        /// </summary>
        /// <param name="name">Имя компонента</param>
        /// <param name="usingOrder">Определяет очередность вызова компонента, если существует несколько компонентов, обрабатывающих один вид сообщений.</param>
        protected MessageServiceComponent(string name, uint? usingOrder = null)
        {
            _name = !string.IsNullOrEmpty(name) ? name : GetType().FullName.GenerateGuid().ToString();
            _order = usingOrder;
        }

        #region CoreComponentBase
        /// <summary>
        /// Выполняется при запуске экземпляра компонента.
        /// </summary>
        protected override void OnStart()
        {
        }

        /// <summary>
        /// Выполняется при остановке экземпляра компонента.
        /// </summary>
        protected override void OnStop()
        {
        }
        #endregion

        #region Виртуальные методы
        /// <summary>
        /// Вызывается во время инициализации компонента.
        /// </summary>
        /// <param name="settings">Содержит сериализованные настройки компонента.</param>
        /// <returns>Если возвращает true, то компонент используется сервисом обработки сообщений.</returns>
        /// <remarks>Дополнительные типы исключений, которые могут возникнуть во время инициализации, могут быть описаны в документации компонента.</remarks>
        protected abstract bool OnInit(string settings);
        #endregion

        #region IMessageServiceComponent
        string IMessageServiceComponent<WebApplication, TMessage>.Name => _name;

        uint IPoolObjectOrdered.OrderInPool => _order ?? 0;

        bool IMessageServiceComponent<WebApplication, TMessage>.Init(string settings)
        {
            return OnInit(settings);
        }
        #endregion
    }
}
