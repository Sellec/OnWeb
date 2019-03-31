using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using System.Web.Mvc;

namespace OnWeb.Core
{
    public class CaptchManager : BaseStatic<Configuration.CoreContext>
    {
        static CaptchManager()
        {
            Journaling.Manager.RegisterJournalTyped<CaptchManager>("Журнал менеджера капчи");
        }

        /**
         * Генерация капчи.
         * */
        public static Tuple<string, int> generateCode()
        {
            var code = DateTime.Now.Microtime().Md5();
            var time = DateTime.Now.Timestamp();

            var rand = new Random(time);
            var number = rand.Next(10000, 99999);
            using (var db = CreateContextScope())
            {
                db.Item.captcha.Add(new TraceWeb.DB.captcha()
                {
                    code = code,
                    number = number,
                    dtime = time
                });
                db.Item.SaveChanges();
            }

            return new Tuple<string, int>(code, number);
        }

        /**
         * Проверка кода капчи
         * */
        public static bool checkCode(string code, int number)
        {
            if (DataManager.check(code)) return false;

            using (var db = CreateContextScope())
            {
                var res = db.Item.captcha.Where(x => x.code == code).FirstOrDefault();
                if (res != null)
                {
                    var result = res.number == number;
                    db.Item.captcha.Delete(res);
                    db.Item.SaveChanges();

                    return result;
                }
            }

            return false;
        }

        /**
         * Новый вариант проверки. Проверяется только номер.
         * */
        public static bool check(int number)
        {
            try
            {
                setError(null);

                using (var db = CreateContextScope())
                {
                    var res = db.Item.captcha.Where(x => x.number == number).FirstOrDefault();
                    if (res == null) throw new Exception("Указано неправильное проверочное число");

                    db.Item.captcha.Delete(res);
                    db.Item.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                setError(ex.Message);
                return false;
            }
        }

        class ReCaptcha2Answer
        {
            [Newtonsoft.Json.JsonProperty("success")]
            public bool Success = false;

            [Newtonsoft.Json.JsonProperty("error-codes")]
            public string[] Errors = null;
        }

        /// <summary>
        /// Проверяет текущий запрос на наличие результата проверки капчи.
        /// </summary>
        /// <exception cref="InvalidOperationException">Возникает в случае, если метод вызван не во время обработки входящего запроса.</exception>
        /// <returns></returns>
        public static bool checkReCaptcha()
        {
            try
            {
                setError(null);

                var recaptchaResponse = string.Empty;

                try
                {
                    recaptchaResponse = HttpContext.Current.Request.Form["g-recaptcha-response"];
                }
                catch (HttpException)
                {
                    throw new InvalidOperationException("Этот метод можно вызывать только в рамках входящего запроса");
                }

                if (string.IsNullOrEmpty(recaptchaResponse)) throw new ArgumentException("В данных, переданных из формы, отсутствует результат проверки капчи.");

                var defaultWebProxy = System.Net.WebRequest.DefaultWebProxy;
                defaultWebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                var client = new System.Net.WebClient()
                {
                    Encoding = Encoding.UTF8,
                    Proxy = defaultWebProxy
                };

                var url = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe", recaptchaResponse);
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
                                    errors.Add("Ошибка проверки - неправильный запрос.");
                                    break;

                                case "invalid-input-secret":
                                    errors.Add("Ошибка проверки - неправильная настройка сайта.");
                                    break;

                                case "missing-input-response":
                                    errors.Add("Ошибка проверки - не передан результат проверки.");
                                    break;

                                case "invalid-input-response":
                                    errors.Add("Ошибка проверки - неправильный результат проверки.");
                                    break;
                            }

                    setError(string.Join("\r\n", errors));
                }

                return answer.Success;
            }
            catch(HandledException ex)
            {
                setError(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Journaling.Manager.RegisterEvent<CaptchManager>(Journaling.EventType.CriticalError, "Ошибка проверки капчи", null, null, ex);
                Debug.WriteLine("checkReCaptcha: " + ex.ToString());
                setError("Возникла ошибка во время проверки результатов капчи.");
                return false;
            }
        }

        /// <summary>
        /// Выводит html-код формы для проверки капчи.
        /// </summary>
        /// <param name="callbackSuccessName"></param>
        /// <param name="callbackFailedName"></param>
        /// <returns></returns>
        public static IHtmlString reRender(string callbackSuccessName = null, string callbackFailedName = null)
        {
            var tags = new List<string>();
            if (!string.IsNullOrEmpty(callbackSuccessName)) tags.Add("data-callback='" + callbackSuccessName + "'");
            if (!string.IsNullOrEmpty(callbackFailedName)) tags.Add("data-expired-callback='" + callbackFailedName + "'");

            return new MvcHtmlString(string.Format(@"
                <script src ='https://www.google.com/recaptcha/api.js' async defer></script>
                <div class='g-recaptcha' data-sitekey='{0}' {1}></div>
            ", ApplicationCore.Instance.Config.Get("reCaptchaSiteKey", ""), string.Join(" ", tags)));
        }

    }
}