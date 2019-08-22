using System.Linq.Expressions;

namespace System.Web.Mvc
{
    /// <summary>
    /// </summary>
    public static class HtmlExtension
    {
        /// <summary>
        /// Возвращает json-представление объекта для проецирования объекта со всеми свойствами в скрипт JavaScript.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MvcHtmlString jsobject(this object obj)
        {
            try
            {
                var str = Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings() {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                });
                return MvcHtmlString.Create(str);

                //var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string jsonString = javaScriptSerializer.Serialize(obj);

                //return MvcHtmlString.Create(jsonString);
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Заменяет символы переноса каретки и новой строки на теги <br /> для представления <paramref name="str"/> как javascript-строки.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static MvcHtmlString java_string(this string str)
        {
            if (str == null) str = "";
            return MvcHtmlString.Create(str.Replace("\r\n", "<br/>")
                                            .Replace("\r", "<br/>")
                                            .Replace("\n", "<br/>"));
        }

        /// <summary>
        /// Заменяет символы переноса каретки и новой строки на теги <br />.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IHtmlString FormatNewline(this string str)
        {
            return MvcHtmlString.Create(str.Replace("\r\n", "<br/>")
                                            .Replace("\r", "<br/>")
                                            .Replace("\n", "<br/>"));
        }

        /// <summary>
        /// Возвращает полный путь свойства, указанного в выражении <paramref name="expression"/>.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string NameOf<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            var dd = ExpressionHelper.GetExpressionText(expression);
            return dd;
        }
    }
}
