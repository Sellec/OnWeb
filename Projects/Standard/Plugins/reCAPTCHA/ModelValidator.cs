using OnUtils.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace OnWeb.Plugins.reCAPTCHA
{
    class ModelValidator : System.Web.Mvc.ModelValidator
    {
        private ApplicationCore _appCore;
        private readonly string _privateKey;

        public ModelValidator(ApplicationCore appCore, string privateKey, ModelMetadata metadata, ControllerContext controllerContext) : base(metadata, controllerContext)
        {
            _appCore = appCore;
            _privateKey = privateKey;
        }

        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            if (container == null && Metadata.ContainerType == null)
            {
                var attribute = Metadata.ModelType.GetCustomAttribute<ModelAttribute>(true);
                if (attribute != null)
                {
                    try
                    {
                        var recaptchaResponse = ControllerContext.HttpContext.Request.Form["g-recaptcha-response"];
                        if (string.IsNullOrEmpty(recaptchaResponse)) return new ModelValidationResult() { Message = "В данных, переданных из формы, отсутствует результат проверки капчи." }.ToEnumerable();

                        var defaultWebProxy = System.Net.WebRequest.DefaultWebProxy;
                        defaultWebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                        var client = new System.Net.WebClient()
                        {
                            Encoding = Encoding.UTF8,
                            Proxy = defaultWebProxy
                        };

                        var url = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", _privateKey, recaptchaResponse);
                        var json = client.DownloadString(url);

                        var answer = Newtonsoft.Json.JsonConvert.DeserializeObject<ReCaptcha2Answer>(json);
                        if (!answer.Success)
                        {
                            var errors = new List<string>();
                            if (answer.Errors != null)
                                foreach (var error in answer.Errors)
                                    switch (error)
                                    {
                                        case "missing-input-secret":
                                            errors.Add("Ошибка reCAPTCHA - неправильный запрос.");
                                            break;

                                        case "invalid-input-secret":
                                            errors.Add("Ошибка reCAPTCHA - неправильная настройка сайта.");
                                            break;

                                        case "missing-input-response":
                                            errors.Add("Ошибка reCAPTCHA - не передан результат проверки.");
                                            break;

                                        case "invalid-input-response":
                                            errors.Add("Ошибка reCAPTCHA - неправильный результат проверки.");
                                            break;

                                        default:
                                            errors.Add("Неизвестная ошибка reCAPTCHA.");
                                            break;
                                    }

                            return new ModelValidationResult() { Message = string.Join("\r\n", errors) }.ToEnumerable();
                        }

                        return Enumerable.Empty<ModelValidationResult>();
                    }
                    catch (Exception ex)
                    {
                        return new ModelValidationResult() { Message = $"Ошибка reCAPTCHA - {ex.Message}." }.ToEnumerable();
                    }
                }
            }

            return Enumerable.Empty<ModelValidationResult>();
        }
    }
}