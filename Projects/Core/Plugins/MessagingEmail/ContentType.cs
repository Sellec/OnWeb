namespace OnWeb.Plugins.MessagingEmail
{
    /// <summary>
    /// Варианты содержимого, передаваемого в сервис отправки электронной почты.
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// Содержимое представляет собой простой текст.
        /// </summary>
        Text,

        /// <summary>
        /// Содержимое представляет собой html-документ или содержит html-теги.
        /// </summary>
        Html
    }
}
