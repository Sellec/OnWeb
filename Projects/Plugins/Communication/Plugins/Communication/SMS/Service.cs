using System;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Plugins.Communication.SMS
{
    using Core.Messaging;
    using Core.Messaging.SMS;

    class Service : ServiceBase<Message>, IService
    {
        public Service() : base("SMS", "SMS".GenerateGuid())
        {
            IsSupportsIncoming = false;
            IsSupportsOutcoming = true;
            IsSupportsCurrentStatusInfo = false;
        }

        #region IService
        bool IService.SendMessage(string phoneTo, string messageText)
        {
            //todo setError(null);

            var phone = PhoneBuilder.ParseString(phoneTo);
            if (!phone.IsCorrect) throw new ArgumentException("Некорректный номер телефона: " + phone.Error, nameof(phoneTo));

            var message = new Message()
            {
                To = new Contact<string>("", phone.ToString()),
                Subject = string.Empty,
                Body = messageText,
                IdMessageType = IdMessageType
            };

            return RegisterMessage(message);
        }

        bool IService.SendToAdmin(string messageText)
        {
            return ((IService)this).SendMessage("+79168841097", messageText);
        }
        #endregion
    }
}
