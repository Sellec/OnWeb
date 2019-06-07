namespace OnWeb.Plugins.MessagingEmail.Model
{
    public class Configuration : CoreBind.Modules.Configuration.SaveModel
    {
        public SmtpServerSettings Amazon { get; set; }

        public SmtpServerSettings Smtp { get; set; }

        public void ApplyConfiguration(Connectors.SmtpServerSettings amazonSES, Connectors.SmtpServerSettings smtp)
        {
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