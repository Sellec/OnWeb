using System;
using System.Web.Mvc;

namespace OnWeb.Modules.reCAPTCHA
{
    /// <summary>
    /// Применяется к классу модели и используется во время валидации модели во входящих данных. Если пользователь не прошел проверку сервиса Google ReCaptcha, то в <see cref="ModelStateDictionary"/> появляется запись с соответствующей ошибкой.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelAttribute : Attribute
    {
    }
}