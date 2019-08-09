using System.Web;

namespace OnWeb.Types
{
    public static class RequestAnswerType
    {
        /// <summary>
        /// Перечисление с типами ожидаемых ответов от сервера.
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// Значение указывает, что запрос содержит некорректный или неподдерживаемый тип требуемого ответа, либо попытка определить тип ответа была выполнена не в контексте запроса.
            /// </summary>
            BadRequest = 0,

            /// <summary>
            /// Требуется обычный ответ для визуализации браузером.
            /// </summary>
            Visual = 2,

            /// <summary>
            /// Требуется ответ в виде стандартного JSON.
            /// </summary>
            Json = 4
        }

        public static Types GetAnswerType()
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                if (HttpContext.Current.Request.Url != null && HttpContext.Current.Request.Url.ToString().Contains("jsonrequestqueryf1F8Dz0"))
                    return Types.Json;

                return Types.Visual;
            }
            else return Types.BadRequest;
        }
    }
}
