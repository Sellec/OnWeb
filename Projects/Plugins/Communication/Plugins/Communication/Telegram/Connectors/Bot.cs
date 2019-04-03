using Newtonsoft.Json;
using OnUtils.Architecture.AppCore;
using OnUtils.Architecture.ObjectPool;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace OnWeb.Plugins.Communication.Telegram.Connectors
{
    using Core;
    using Core.Messaging;
    using Core.Messaging.Connectors;
    using Journaling = Core.Journaling;

    class Bot : CoreComponentBase<ApplicationCore>, IConnectorBase<Message>, IDisposable
    {
        private TelegramBotClient _client = null;
        private global::Telegram.Bot.Types.User _botUser = null;
        private Dictionary<string, long> _usersChatsList = new Dictionary<string, long>();
        private DateTime _lastDBReadTime = DateTime.MinValue;

        #region CoreComponentBase
        protected sealed override void OnStart()
        {
        }

        protected sealed override void OnStop()
        {
            var client = _client;
            _client = null;
        }
        #endregion

        #region IConnectorBase<Message>
        /// <summary>
        /// См. <see cref="IConnectorBase{TMessage}.Init(string)"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Возникает, если коннектор уже был инициализирован.</exception>
        bool IConnectorBase<Message>.Init(string connectorSettings)
        {
            if (_client != null) throw new InvalidOperationException("Коннектор уже инициализирован.");

            try
            {
                var settings = !string.IsNullOrEmpty(connectorSettings) ? JsonConvert.DeserializeObject<ConnectorSettings>(connectorSettings) : new ConnectorSettings();
                if (string.IsNullOrEmpty(settings.TelegramBotID)) throw new Exception("Не указан идентификатор бота.");

                var client = new TelegramBotClient(settings.TelegramBotID);

                var getME = client.GetMeAsync().WaitAndReturn();
                _botUser = getME;

                _client = client;

                return true;
            }
            catch (Exception)
            {
                // todo здесь должна быть возможность писать в журнал. Пока не реализовано - делаем throw дальше, попадет в общий журнал.
                throw;
            }
        }

        public void UpdateChatList()
        {
            try
            {
                if ((DateTime.Now - _lastDBReadTime).TotalMinutes >= 5)
                {
                    using (var db = new UnitOfWork<DB.TelegramContacts>())
                    {
                        db.Repo1.ForEach(x => {
                            long id = 0;
                            if (long.TryParse(x.contactID, out id))
                            {
                                _usersChatsList[x.contactName] = id;
                            }
                        });
                    }
                    _lastDBReadTime = DateTime.Now;
                }

                var listNew = new List<DB.TelegramContacts>();
                var updates = getClient().GetUpdatesAsync().WaitAndReturn();
                foreach (var upd in updates)
                {
                    if (upd.Type == global::Telegram.Bot.Types.Enums.UpdateType.Message)
                    {
                        if (upd.Message.From != null)
                        {
                            if (!_usersChatsList.Any(x => x.Value == upd.Message.From.Id)) listNew.Add(new DB.TelegramContacts() { contactID = upd.Message.From.Id.ToString(), contactName = upd.Message.From.Username });
                            _usersChatsList[upd.Message.From.Username] = upd.Message.From.Id;
                        }
                    }
                }

                if (listNew.Count > 0)
                {
                    using (var db = new UnitOfWork<DB.TelegramContacts>())
                    {
                        db.Repo1.Add(listNew.ToArray());
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        void IConnectorBase<Message>.Send(MessageProcessed<Message> message, IMessagingService service)
        {
            try
            {
                var msg = "";
                if (!string.IsNullOrEmpty(message.Message.Subject)) msg += $"Тема: {message.Message.Subject}\r\n\r\n";
                msg += message.Message.Body?.ToString();
                msg = msg.Truncate(0, 4096);

                var username = message.Message.To.ContactData?.Replace("@", "");

                if (!_usersChatsList.ContainsKey(username))
                {
                    throw new HandledException($"Пользователь '{username}' не найден в списке контактов бота '{_botUser.Username}'.");
                }

                var d = getClient().SendTextMessageAsync(_usersChatsList[username], msg.Trim()).WaitAndReturn();
                var dd = d;

                message.IsHandled = true;
                message.IsSuccess = true;
            }
            catch (HandledException ex)
            {
                service.RegisterServiceEvent(Journaling.EventType.Error, "Telegram - ошибка отправки сообщения", ex.Message, ex.InnerException);
                message.IsError = false;
                message.ErrorText = ex.Message;
            }
            catch (Exception ex)
            {
                service.RegisterServiceEvent(Journaling.EventType.Error, "Telegram - необработанная ошибка отправки сообщения", null, ex);
                Debug.WriteLine(ex.ToString());
                message.IsError = false;
                message.ErrorText = "Необработанная ошибка во время отправки сообщения";
            }
        }

        private TelegramBotClient getClient()
        {
            if (_client == null) throw new InvalidOperationException("Коннектор не был корректно инициализирован.");
            return _client;
        }

        public string ConnectorName
        {
            get => "Telegram bot";
        }
        #endregion

        void IDisposable.Dispose() => OnStop();

        uint IPoolObjectOrdered.OrderInPool { get; } = 20;
    }
}

namespace System
{
    static class TasksExtensions
    {
        public static TResult WaitAndReturn<TResult>(this Task<TResult> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}
