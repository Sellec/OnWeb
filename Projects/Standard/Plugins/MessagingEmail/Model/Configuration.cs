using System.ComponentModel.DataAnnotations;

namespace OnWeb.Plugins.MessagingEmail.Model
{
    public class Configuration : CoreBind.Modules.Configuration.SaveModel
    {
        [Display(Name = "Использовать Amazon SES для отправки почты?")]
        public bool IsUseAmazonSES { get; set; }

        public SmtpServerSettings Amazon { get; set; }

        [Display(Name = "Использовать отдельный SMTP-сервер для отправки почты?")]
        public bool IsUseSmtp { get; set; }

        public SmtpServerSettings Smtp { get; set; }

        public void ApplyConfiguration(Connectors.SmtpServerSettings amazonSES, Connectors.SmtpServerSettings smtp)
        {
            IsUseAmazonSES = amazonSES != null;
            IsUseSmtp = smtp != null;

            Amazon = new SmtpServerSettings()
            {
                Server = amazonSES?.Server?.ToString(),
                Login = amazonSES?.Login,
                Password = amazonSES?.Password
            };

            Smtp = new SmtpServerSettings()
            {
                Server = smtp?.Server?.ToString(),
                Login = smtp?.Login,
                Password = smtp?.Password
            };


        }
    }
}