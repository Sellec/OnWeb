namespace OnWeb.Plugins.Register.Model
{
    using Core.DB;

    /// <summary>
    /// Содержит информацию о зарегистрированном пользователе.
    /// </summary>
    public class RegisterNotificationConfirm
    {
        /// <summary>
        /// Данные пользователя.
        /// </summary>
        public User Data { get; set; }

        /// <summary>
        /// Код подтверждения для самостоятельной активации учетной записи.
        /// </summary>
        public string ConfirmationCode { get; set; }
    }
}