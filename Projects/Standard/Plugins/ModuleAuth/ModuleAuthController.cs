using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Auth
{
    using Core.Configuration;
    using Core.DB;
    using Core.Items;
    using Core.Modules;
    using CoreBind.Modules;

    public class ModuleAuthController : ModuleController<ModuleAuth>
    {
        public ActionResult Index()
        {
            if (Module.IsNeededAnyUserToRegister()) return Redirect<Register.ModuleRegisterController>(x => x.Register());

            this.assign("authorized", UserManager.Instance.isAuthorized);
            this.assign("result", "");

            return display("login.cshtml", new Design.Model.Login());
        }

        [ModuleAction("unauthorized")]
        public ActionResult UnauthorizedAccess(string RedirectedFrom = null)
        {
            if (Module.IsNeededAnyUserToRegister()) return Redirect<Register.ModuleRegisterController>(x => x.Register());

            var redirect = UserManager.Instance.AuthorizationRedirectUrl;

            this.assign("authorized", UserManager.Instance.isAuthorized);

            return display("unauthorizedAccess.cshtml");
        }

        [ModuleAction("login")]
        public virtual ActionResult Login(Model.AuthLoginData model)
        {
            var message = "";

            try
            {
                if (UserManager.Instance.isAuthorized) throw new Exceptions.BehaviourException("Вы уже авторизованы!");

                if (string.IsNullOrEmpty(model.login)) throw new Exceptions.BehaviourException("Некорректно введен логин!");
                if (string.IsNullOrEmpty(model.pass)) throw new Exceptions.BehaviourException("Некорректно введен пароль!");

                var phone = PhoneBuilder.ParseString(model.login);
                if (!model.login.isEmail() && !phone.IsCorrect) throw new Exceptions.BehaviourException("Неправильно введен логин.");

                if (!IsReCaptchaValid) throw new Exceptions.BehaviourException(CaptchManager.getError());

                var result = UserManager.Instance.login(0, model.login, model.pass);

                if (result == UserManager.eAuthResult.Success)
                {
                    //message = "Авторизация прошла успешно!";
                }
                else throw new Exceptions.BehaviourException(UserManager.Instance.getError());
            }
            catch(Exceptions.BehaviourException ex)
            {
                Module.RegisterEvent(Journaling.EventType.Warning, "Ошибка авторизации", ex.Message, ex.InnerException);
                message = ex.Message;
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(Journaling.EventType.Warning, "Ошибка авторизации - непредвиденная", ex.Message, ex.InnerException);
                message = "Неожиданная ошибка во время авторизации. Попробуйте еще раз или обратитесь в техническую поддержку.";
            }

            if (!ModelState.IsValid || !string.IsNullOrEmpty(message)) this.RegisterEventInvalidModel("Форма авторизации", ignoreParamsKeys: new List<string>() { nameof(model.pass) });

            if (UserManager.Instance.isAuthorized)
            {
                if (!string.IsNullOrEmpty(model?.urlFrom) && Url.IsLocalUrl(model.urlFrom)) return new RedirectResult(model.urlFrom, false);

                var redirect = UserManager.Instance.AuthorizationRedirectUrl;
                if (!string.IsNullOrEmpty(redirect)) return new RedirectResult(redirect, false);
            }

            this.assign("authorized", UserManager.Instance.isAuthorized);

            return this.display("login.cshtml", new Design.Model.Login() { Result = message });
        }

        [HttpPost]
        public ActionResult LoginJson(Model.AuthLoginData model)
        {
            var success = false;
            var message = "";

            try
            {
                if (UserManager.Instance.isAuthorized) throw new Exceptions.BehaviourException("Вы уже авторизованы!");

                if (string.IsNullOrEmpty(model.login)) throw new Exceptions.BehaviourException("Некорректно введен логин!");
                if (string.IsNullOrEmpty(model.pass)) throw new Exceptions.BehaviourException("Некорректно введен пароль!");

                var phone = PhoneBuilder.ParseString(model.login);
                if (!model.login.isEmail() && !phone.IsCorrect) throw new Exceptions.BehaviourException("Неправильно введен логин.");

                var result = UserManager.Instance.login(0, model.login, model.pass);

                if (result == UserManager.eAuthResult.Success)
                {
                    message = "Авторизация прошла успешно!";
                    success = true;
                }
                else throw new Exceptions.BehaviourException(UserManager.Instance.getError());
            }
            catch (Exceptions.BehaviourException ex)
            {
                Module.RegisterEvent(Journaling.EventType.Warning, "Ошибка авторизации", ex.Message, ex.InnerException);
                message = ex.Message;
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(Journaling.EventType.Warning, "Ошибка авторизации - непредвиденная", ex.Message, ex.InnerException);
                message = "Неожиданная ошибка во время авторизации. Попробуйте еще раз или обратитесь в техническую поддержку.";
            }

            if (!ModelState.IsValid || !string.IsNullOrEmpty(message)) this.RegisterEventInvalidModel("Форма авторизации JSON", ignoreParamsKeys: new List<string>() { nameof(model.pass) });

            return this.ReturnJson(success, message, new
            {
                authorized = UserManager.Instance.isAuthorized,
                admin = this.Module.isAdmin()
            });
        }

        public ActionResult logout()
        {
            UserManager.Instance.logout();
            UserManager.Instance.loginFromSession();

            return Redirect("/");
        }

        public ActionResult logoutJson()
        {
            var success = false;
            var message = "";

            try
            {
                UserManager.Instance.logout();
                success = true;
                message = "Выход прошел успешно.";
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return this.ReturnJson(success, message, UserManager.AuthorizationRedirect);
        }

        [ModuleAction("restore")]
        public ActionResult PasswordRestore()
        {
            if (Module.IsNeededAnyUserToRegister()) return Redirect<Register.ModuleRegisterController>(x => x.Register());

            if (UserManager.Instance.isAuthorized) return RedirectToAction(nameof(Login));
            return display("PasswordRestore.cshtml", new Model.PasswordRestore());
        }

        [ModuleAction("restore2")]
        public JsonResult PasswordRestoreSend(Model.PasswordRestore model = null)
        {
            var answer = JsonAnswer<string>();
            try
            {
                if (!IsReCaptchaValid) throw new Exceptions.BehaviourException("Докажите, что вы не робот!");

                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.email) && !string.IsNullOrEmpty(model.phone)) ModelState.AddModelError(nameof(model.EmailOrPhone), "Следует указать либо адрес электронной почты, либо номер телефона.");
                }

                var isPhone = !string.IsNullOrEmpty(model.phone);

                if (ModelState.IsValid)
                {
                    if (!isPhone)
                    {
                        if (!ApplicationCore.Instance.Config.userAuthorizeAllowed.In(UserManager.eUserAuthorizeAllowed.EmailAndPhone, UserManager.eUserAuthorizeAllowed.OnlyEmail)) ModelState.AddModelError(nameof(model.email), "К сожалению, в данный момент авторизация через адрес электронной почты отключена, восстановление пароля таким способом невозможно.");
                    }
                    else if (isPhone)
                    {
                        if (!ApplicationCore.Instance.Config.userAuthorizeAllowed.In(UserManager.eUserAuthorizeAllowed.EmailAndPhone, UserManager.eUserAuthorizeAllowed.OnlyPhone)) ModelState.AddModelError(nameof(model.phone), "К сожалению, в данный момент авторизация через номер телефона отключена, восстановление пароля таким образом невозможно.");
                    }
                }

                if (ModelState.IsValid)
                {
                    var sql = from p in DB.Users
                              where (!isPhone && p.email.Length > 0 && p.email.ToLower() == model.email.ToLower()) || (isPhone && p.phone.Length > 0 && p.phone.ToLower() == model.phone.ToLower())
                              select p;

                    var user = sql.FirstOrDefault();

                    if (user == null)
                    {
                        if (!isPhone)
                            throw new Exceptions.BehaviourException("Указанный Email-адрес не найден на сайте!");
                        else
                            throw new Exceptions.BehaviourException("Указанный номер телефона не найден на сайте!");
                    }
                    else
                    {
                        var codeType = !isPhone ? "email" : "phone";

                        using (var scope = DB.CreateScope())
                        {
                            if (!isPhone)
                            {
                                var code = DateTime.Now.Microtime().Md5();
                                DB.PasswordRemember.Add(new DB.PasswordRemember() { user_id = user.id, code = code });

                                var send = Messaging.Manager.Email.sendMailFromSite(
                                    user.Caption,
                                    user.email,
                                    "Восстановление пароля на сайте",
                                    this.displayToVar("PasswordRestoreNotificationEmail.cshtml", new Design.Model.PasswordRestoreSend() { User = user, Code = code, CodeType = codeType })
                                );
                                if (!send) throw new Exceptions.BehaviourException("Ошибка во время отправки письма на указанный Email-адрес", new Exception(Messaging.Manager.Email.getError(), Messaging.Manager.Email.getErrorException()));
                            }
                            else
                            {
                                var code = TraceCore.Utils.StringsHelper.GenerateRandomString("0123456789", 4);
                                DB.PasswordRemember.Add(new DB.PasswordRemember() { user_id = user.id, code = code });

                                var send = Messaging.Manager.SMS.sendMessage(user.phone, "Код восстановления пароля: " + code);
                                if (!send) throw new Exceptions.BehaviourException("Ошибка во время отправки сообщения с кодом на указанный номер телефона", new Exception(Messaging.Manager.SMS.getError(), Messaging.Manager.SMS.getErrorException()));
                            }

                            DB.SaveChanges();
                            scope.Commit();
                        }

                        answer.Data = codeType;
                        if (!isPhone)
                            answer.FromSuccess("На указанный адрес электронной почты был отправлен код подтверждения.");
                        else
                            answer.FromSuccess("На указанный номер телефона был отправлен код подтверждения.");
                    }
                }
            }
            catch (Exceptions.BehaviourException ex)
            {
                Module.RegisterEvent(Journaling.EventType.Info, "Ошибка восстановления пароля", ex.Message, ex.InnerException);
                answer.FromFail(ex.Message);
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(Journaling.EventType.Error, "Ошибка восстановления пароля", "Необработанная ошибка", ex);
                answer.FromFail("Возникла ошибка во время восстановления пароля");
            }

            if (!ModelState.IsValid) this.RegisterEventInvalidModel("Форма восстановления пароля - шаг 1");

            return ReturnJson(answer);
        }

        [ModuleAction("restore3")]
        public ActionResult PasswordRestoreVerify(string Code = null, string CodeType = null)
        {
            if (Module.IsNeededAnyUserToRegister()) return Redirect<Register.ModuleRegisterController>(x => x.Register());

            return this.display("PasswordRestoreVerify.cshtml", new Design.Model.PasswordRestoreVerify() { Code = Code?.Truncate(0, 32), CodeType = CodeType?.Truncate(0, 6) });
        }

        [ModuleAction("restore4")]
        public ActionResult PasswordRestorSave(Model.PasswordRestoreSave model = null)
        {
            var answer = JsonAnswer<string>();
            try
            {
                if (!IsReCaptchaValid) throw new Exceptions.BehaviourException("Докажите, что вы не робот!");

                if (ModelState.IsValid)
                {
                    var res = (from p in DB.PasswordRemember
                               join u in DB.Users on p.user_id equals u.id
                               where p.code == model.Code
                               select new { Password = p, User = u }).FirstOrDefault();

                    if (res == null) ModelState.AddModelError(nameof(model.Code), "Некорректный код подтверждения.");
                    else
                    {
                        var symbols = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                        var salt = "";

                        var rand = new Random();
                        for (int i = 0; i < 5; i++)
                        {
                            var index = rand.Next(0, symbols.Length - 1);
                            salt = salt + symbols[index];
                        }

                        using (var scope = new System.Transactions.TransactionScope())
                        {
                            DB.PasswordRemember.Delete(res.Password);

                            res.User.password = UserManager.hashPassword(model.Password);
                            res.User.salt = salt;

                            DB.SaveChanges();

                            scope.Complete();
                        }

                        answer.FromSuccess("Новый пароль был сохранен.");
                    }
                }
            }
            catch (Exceptions.BehaviourException ex)
            {
                Module.RegisterEvent(Journaling.EventType.Info, "Ошибка восстановления пароля при сохранении", ex.Message, ex.InnerException);
                answer.FromFail(ex.Message);
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(Journaling.EventType.Error, "Ошибка восстановления пароля при сохранении", "Необработанная ошибка", ex);
                answer.FromFail("Возникла ошибка во время сохранения пароля");
            }

            if (!ModelState.IsValid) this.RegisterEventInvalidModel("Форма восстановления пароля - шаг 2");

            return ReturnJson(answer);
        }


    }

}