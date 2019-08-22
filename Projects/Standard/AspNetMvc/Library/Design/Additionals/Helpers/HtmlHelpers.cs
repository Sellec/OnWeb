using System.Collections.Generic;
using System.Text;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelpersCustom
    {
        public static string WordWrap(this string str, int wrapLength, string wrapSymbols)
        {
            int pos, next;
            StringBuilder sb = new StringBuilder();

            // Lucidity check
            if (wrapLength < 1) return str;

            // Parse each line of text
            for (pos = 0; pos < str.Length; pos = next)
            {
                // Find end of line
                int eol = str.IndexOf(wrapSymbols, pos);

                if (eol == -1) next = eol = str.Length;
                else next = eol + wrapSymbols.Length;

                // Copy this line of text, breaking into smaller lines as needed
                if (eol > pos)
                {
                    do
                    {
                        int len = eol - pos;

                        if (len > wrapLength) len = BreakLine(str, pos, wrapLength);

                        sb.Append(str, pos, len);
                        sb.Append(wrapSymbols);

                        // Trim whitespace following break
                        pos += len;

                        while (pos < eol && Char.IsWhiteSpace(str[pos])) pos++;

                    }
                    while (eol > pos);
                }
                else sb.Append(wrapSymbols); // Empty line
            }

            return sb.ToString();
        }

        /// <summary>
        /// Locates position to break the given line so as to avoid
        /// breaking words.
        /// </summary>
        /// <param name="text">String that contains line of text</param>
        /// <param name="pos">Index where line of text starts</param>
        /// <param name="max">Maximum line length</param>
        /// <returns>The modified line length</returns>
        public static int BreakLine(string text, int pos, int max)
        {
            // Find last whitespace in line
            int i = max - 1;
            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i])) i--;
            if (i < 0) return max; // No whitespace found; break at maximum length
                                   // Find start of whitespace
            while (i >= 0 && Char.IsWhiteSpace(text[pos + i])) i--;
            // Return length of text before whitespace
            return i + 1;
        }

        /// <summary>
        /// Превращает обычные переносы строк в html-вариант.
        /// </summary>
        public static MvcHtmlString Nl2Br(this HtmlHelper html, string text)
        {
            if (text == null) text = "";
            return MvcHtmlString.Create(text.Replace("\n", "<br />\n"));
        }

        /// <summary>
        /// Возвращает json-представление объекта <paramref name="obj"/> для проецирования объекта со всеми свойствами в скрипт JavaScript
        /// </summary>
        /// <param name="ignoreFieldsAndProperties">Указывает, какие свойства и поля объекта следует исключить при формировании JSON.</param>
        /// <returns></returns>
        public static MvcHtmlString AsJSON(this HtmlHelper html, object obj, params string[] ignoreFieldsAndProperties)
        {
            try
            {
                var str = string.Empty;
                if (obj == null) str = "null";
                else
                {
                    str = Newtonsoft.Json.JsonConvert.SerializeObject(obj, new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                        TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                        TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
                        ContractResolver = new OnUtils.Utils.JsonContractResolver() { IgnorePropertiesAndFields = ignoreFieldsAndProperties }
                    });
                }

                return MvcHtmlString.Create(str);

                //var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string jsonString = javaScriptSerializer.Serialize(obj);

                //return MvcHtmlString.Create(jsonString);
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// Выводит тег <option></option> с указанными value и текстом.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static MvcHtmlString Option(this HtmlHelper html, object value, string text, object htmlAttributes = null)
        {
            if (value == null) return MvcHtmlString.Empty;

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var tagBuilder = new TagBuilder("option");
            tagBuilder.Attributes.Add("value", value.ToString());
            foreach (var attr in attrs) tagBuilder.MergeAttribute(attr.Key, attr.Value.ToString());
            tagBuilder.SetInnerText(text);
            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }

        //public static IEnumerable<SelectListItem> EnumFriendlyNames<TEnum>(this HtmlHelper html, TEnum valueExample = default(TEnum)) where TEnum : struct, IConvertible
        //{
        //    var collection = OnUtils.Utils.TypeHelper.EnumFriendlyNames<TEnum>();

        //    var result = collection.Select(x => new SelectListItem()
        //    {
        //        Text = x.Value,
        //        Value = x.Key.ToString(),
        //    });

        //    return result;
        //}

        public static MvcHtmlString DisplayEnum<TEnum>(this HtmlHelper html, TEnum objectValue = default(TEnum)) where TEnum : struct, IConvertible
        {
            return new MvcHtmlString(objectValue.ToStringFriendly());
        }

        /// <summary>
        /// Отображает сообщение об ошибке для всей модели.
        /// </summary>
        public static MvcHtmlString ValidationMessageForModel<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return ValidationMessageForModel<TModel>(htmlHelper, null);
        }

        /// <summary>
        /// Отображает сообщение об ошибке для всей модели.
        /// </summary>
        public static MvcHtmlString ValidationMessageForModel<TModel>(this HtmlHelper<TModel> htmlHelper, object htmlAttributes)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            return ValidationMessageForModel<TModel>(htmlHelper, attributes);
        }

        /// <summary>
        /// Отображает сообщение об ошибке для всей модели.
        /// </summary>
        public static MvcHtmlString ValidationMessageForModel<TModel>(this HtmlHelper<TModel> htmlHelper, IDictionary<string, object> htmlAttributes)
        {
            htmlHelper.ViewData.ModelState["__entire_model__"] = new ModelState();

            var attributes = htmlAttributes == null ? new Dictionary<string, object>() : new Dictionary<string, object>(htmlAttributes);
            attributes["id"] = "__entire_model__" + "_validationMessage";
            return htmlHelper.ValidationMessage("__entire_model__", attributes);
        }

    }
}