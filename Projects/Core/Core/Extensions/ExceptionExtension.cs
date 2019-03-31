using System.Linq;

namespace System
{
    /// <summary>
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// Возвращает содержимое исключения в более удобочитаемом виде.
        /// </summary>
        public static string GetMessageExtended(this Exception ex)
        {
            return GetMessageExtended(ex, 0);
        }

        private static string GetMessageExtended(this Exception ex, int level)
        {
            if (ex == null) return string.Empty;

            if (ex is AggregateException)
            {
                var rep = string.Concat(Enumerable.Repeat("  ", level));
                var exc = (ex as AggregateException).InnerExceptions.Select(x => x.GetMessageExtended(level + 1));
                var exc2 = rep + " - " + string.Join("\r\n" + rep + " - ", exc);
                return ex.Message + "\r\n" + exc2;
            }
            else
            {
                return ex.Message;
            }
        }
    }
}
