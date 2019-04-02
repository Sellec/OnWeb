using System;

namespace OnWeb.Plugins.Communication.Telegram
{
    using Core.Messaging;

    class Service : ServiceBase<Message>, IService
    {
        public Service() : base("Telegram", "Telegram".GenerateGuid())
        {
            IsSupportsOutcoming = true;
            IsSupportsIncoming = false;
            IsSupportsCurrentStatusInfo = false;
        }

        protected override void OnBeforeExecuteOutcoming(int messagesCount)
        {
            if (messagesCount > 0)
            {
                foreach (var connector in GetConnectors())
                {
                    if (connector is Connectors.Bot)
                    {
                        (connector as Connectors.Bot).UpdateChatList();
                    }
                }
            }
        }

        #region Отправка
        /// <summary>
        /// Отправка сообщения от имени бота указанному адресату.
        /// </summary>
        /// <param name="username">Логин пользователя в Telegram.</param>
        /// <param name="message">Текст сообщения.</param>
        /// <returns>Результат регистрации сообщения в системе обмена сообщениями.</returns>
        public bool Send(string username, string message)
        {
            //todo setError(null);

            var msg = new Message()
            {
                To = new Contact<string>(username, username),
                Body = message,
            };

            return RegisterMessage(msg);
        }

        bool IService.SendToAdmin(string message)
        {
            var adminUserName = AppCore.Get<ModuleCommunication>().GetConfiguration<ModuleConfiguration>().AdminUserName;
            if (string.IsNullOrEmpty(adminUserName)) throw new Exception("Не задано имя администратора в Telegram.");
            return Send(adminUserName, message);
        }

        void ICriticalMessagesReceiver.SendToAdmin(string subject, string body)
        {
            ((IService)this).SendToAdmin($"{subject}\r\n{body}");
        }
        #endregion

    }
}
