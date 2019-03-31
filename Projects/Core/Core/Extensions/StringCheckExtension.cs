namespace System
{
    /// <summary>
    /// </summary>
    public static class StringCheckExtension
    {
        private static bool checkPattern(string data, string pattern)
        {
            if (string.IsNullOrEmpty(data)) return false;

            var result = System.Text.RegularExpressions.Regex.IsMatch(data, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return result;
        }

        /// <summary>
        /// Проверяет, содержит ли строка дробное или целое число.
        /// </summary>
        public static bool isFloat(this string data)
        {
            if (string.IsNullOrEmpty(data)) return false;

            float val;
            return float.TryParse(data, out val);
        }

        public static bool isEmail(this string data)
        {
            return checkPattern(data, "[\\.\\-_A-Za-z0-9\\w]+?@[\\.\\-A-Za-z0-9\\w]+?\\.[\\wa-z0-9]{2,6}");
            //return checkPattern(data, "/^[\\.\\-_A-Za-z0-9\\w]+?@[\\.\\-A-Za-z0-9\\w]+?\\.[\\wa-z0-9]{2,6}$/iu");
        }

        public static bool isTextOnly(this string data, bool allow_multiline_text = true)
        {
            return checkPattern(data, "([a-zA-Z0-9а-яА-ЯЁёеЕ\\ `\\-~!@#\\$%\\^&\\*\\(\\)_\\+\"№;\\%:\\?\\{\\}\\|\\'\\[\\],\\.\\/\\>\\<])*");
        }

        public static bool isOneStringTextOnly(this string data)
        {
            return isTextOnly(data, false);
        }

   }
}
