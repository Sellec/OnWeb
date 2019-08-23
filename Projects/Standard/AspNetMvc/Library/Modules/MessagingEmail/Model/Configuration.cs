using System.ComponentModel.DataAnnotations;

namespace OnWeb.Modules.MessagingEmail.Model
{
    public class Configuration : Core.Modules.Configuration.SaveModel
    {
        [Display(Name = "Использовать отдельный SMTP-сервер для отправки почты?")]
        public bool IsUseSmtp { get; set; }

        public SmtpServerSettings Smtp { get; set; }

        public void ApplyConfiguration(MessageHandlers.SmtpServerSettings smtp)
        {
            IsUseSmtp = smtp != null;

            Smtp = new SmtpServerSettings()
            {
                Server = smtp?.Server?.ToString(),
                IsSecure = smtp?.IsSecure ?? false,
                Port = smtp?.Port,
                Login = smtp?.Login,
                Password = smtp?.Password
            };


        }
    }
}