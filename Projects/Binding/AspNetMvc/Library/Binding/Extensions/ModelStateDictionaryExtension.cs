using System.Collections.Generic;
using System.Linq;

namespace System.Web.Mvc
{
    /// <summary>
    /// </summary>
    public static class ModelStateDictionaryExtension
    {
        /// <summary>
        /// Определяет, существует ли в информации о проверке модели данных указанный ключ <paramref name="key"/> и проверяет, 
        /// не было ли найдено ошибок валидации для данного ключа.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="key"></param>
        /// <returns>Возвращает true, если данные формы для ключа <paramref name="key"/> прошли валидацию.</returns>
        public static bool ContainsKeyCorrect(this ModelStateDictionary modelState, string key)
        {
            if (modelState.ContainsKey(key)) return modelState[key].Errors.Count == 0;
            return false;
        }

        /// <summary>
        /// Определяет, существует ли в информации о проверке модели данных указанный ключ <paramref name="key"/> и проверяет, 
        /// были ли найдены ошибки валидации для данного ключа.
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="key"></param>
        /// <returns>Возвращает true, если данные формы для ключа <paramref name="key"/> не прошли валидацию.</returns>
        public static bool ContainsKeyError(this ModelStateDictionary modelState, string key)
        {
            if (modelState.ContainsKey(key)) return modelState[key].Errors.Count > 0;
            return false;
        }

        public static void AddModelError(this ModelStateDictionary modelState, string key, string errorMessage, params object[] _args)
        {
            if (_args != null && _args.Length > 0) errorMessage = string.Format(errorMessage, _args);
            modelState.AddModelError(key, errorMessage);
        }

        public static Dictionary<string, string[]> ToErrorList(this ModelStateDictionary modelState)
        {
            var func = new Func<ModelError, string>(state =>
            {
                var str = string.Empty;
                if (!string.IsNullOrEmpty(state.ErrorMessage)) str += state.ErrorMessage;
                if (state.Exception != null) str += (!string.IsNullOrEmpty(str) ? " " : "") + state.Exception.Message;

                return str;
            });

            var d = modelState
                .Where(kvp => kvp.Value.Errors.Count > 0)
                .ToDictionary(kvp => String.IsNullOrWhiteSpace(kvp.Key) ? String.Empty : kvp.Key,
                              kvp => kvp.Value.Errors.Select(e => func(e)).ToArray());

            return d;
        }
    }
}
