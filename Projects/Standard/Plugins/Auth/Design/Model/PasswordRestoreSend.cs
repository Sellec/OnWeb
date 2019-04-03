namespace OnWeb.Plugins.Auth.Design.Model
{
    using Core.DB;

    public class PasswordRestoreSend
    {
        public User User { get; set; }

        public string Code { get; set; }

        public string CodeType { get; set; }
    }
}