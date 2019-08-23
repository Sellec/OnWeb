using OnUtils.Application;
using OnUtils.Application.Journaling;
using OnUtils.Application.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Modules.Auth
{
    using Core.DB;
    using Core.Modules;
    using Core.Users;
    using Exceptions;
    using MessagingEmail;

    public class ModuleAuthController : ModuleControllerUser<ModuleAuth>
    {
        public override ActionResult Index()
        {
            if (Module.IsNeededAnyUserToRegister()) return Redirect<Register.ModuleRegister, Register.ModuleRegisterController>(x => x.Register());

            this.assign("authorized", !AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest);
            this.assign("result", "");

            return display("login.cshtml", new Design.Model.Login());
        }

        [ModuleAction("unauthorized")]
        public ActionResult UnauthorizedAccess(string RedirectedFrom = null)
        {
            if (Module.IsNeededAnyUserToRegister()) return Redirect<Register.ModuleRegister, Register.ModuleRegisterController>(x => x.Register());

            this.assign("authorized", !AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest);

            return display("unauthorizedAccess.cshtml");
        }

        [ModuleAction("login")]
        public virtual ActionResult Login(Model.AuthLoginData model)
        {
            var message = "";

            try
            {
                if (!AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest) throw new BehaviourException("Вы уже авторизованы!");

                if (string.IsNullOrEmpty(model.login)) throw new BehaviourException("Некорректно введен логин!");
                if (string.IsNullOrEmpty(model.pass)) throw new BehaviourException("Некорректно введен пароль!");

                if (ModelState.IsValid)
                {
                    var result = AppCore.Get<WebUserContextManager>().CreateUserContext(model.login, model.pass, out var userContext, out var resultReason);
                    if (result == eAuthResult.Success)
                    {
                        AppCore.Get<Binding.Providers.SessionBinder>().BindUserContextToRequest(userContext);
                        AppCore.GetUserContextManager().SetCurrentUserContext(userContext);
                        //message = "Авторизация прошла успешно!";
                    }
                    else throw new BehaviourException(resultReason);
                }
            }
            catch (BehaviourException ex)
            {
                Module.RegisterEvent(EventType.Warning, "Ошибка авторизации", ex.Message, ex.InnerException);
                message = ex.Message;
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(EventType.Warning, "Ошибка авторизации - непредвиденная", ex.Message, ex.InnerException);
                message = "Неожиданная ошибка во время авторизации. Попробуйте еще раз или обратитесь в техническую поддержку.";
            }

            if (!ModelState.IsValid || !string.IsNullOrEmpty(message)) this.RegisterEventInvalidModel("Форма авторизации", ignoreParamsKeys: new List<string>() { nameof(model.pass) });

            if (!AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest)
            {
                if (!string.IsNullOrEmpty(model?.urlFrom) && Url.IsLocalUrl(model.urlFrom)) return new RedirectResult(model.urlFrom, false);

                var redirect = Module.GetRememberedUserContextRequestedAddressWhenRedirectedToAuthorization();
                if (redirect != null) return new RedirectResult(redirect.ToString(), false);
            }

            this.assign("authorized", !AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest);

            return View("login.cshtml", new Design.Model.Login() { Result = message });
        }

        [HttpPost]
        public ActionResult LoginJson(Model.AuthLoginData model)
        {
            var success = false;
            var message = "";

            try
            {
                if (!AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest) throw new BehaviourException("Вы уже авторизованы!");

                if (string.IsNullOrEmpty(model.login)) throw new BehaviourException("Некорректно введен логин!");
                if (string.IsNullOrEmpty(model.pass)) throw new BehaviourException("Некорректно введен пароль!");

                if (ModelState.IsValid)
                {
                    var result = AppCore.Get<WebUserContextManager>().CreateUserContext(model.login, model.pass, out var userContext, out var resultReason);
                    switch (result)
                    {
                        case eAuthResult.Success:
                            AppCore.Get<Binding.Providers.SessionBinder>().BindUserContextToRequest(userContext);
                            AppCore.GetUserContextManager().SetCurrentUserContext(userContext);
                            message = "Авторизация прошла успешно!";
                            success = true;
                            break;

                        case eAuthResult.AuthDisabled:
                        case eAuthResult.AuthMethodNotAllowed:
                        case eAuthResult.BlockedUntil:
                        case eAuthResult.MultipleFound:
                        case eAuthResult.NothingFound:
                        case eAuthResult.RegisterDecline:
                        case eAuthResult.RegisterNeedConfirmation:
                        case eAuthResult.RegisterWaitForModerate:
                        case eAuthResult.WrongAuthData:
                        case eAuthResult.Disabled:
                        case eAuthResult.YetAuthorized:
                            ModelState.AddModelError(nameof(model.login), resultReason);
                            break;

                        case eAuthResult.WrongPassword:
                            ModelState.AddModelError(nameof(model.pass), resultReason);
                            break;

                        default:
                            throw new BehaviourException(resultReason);
                    }
                }
            }
            catch (BehaviourException ex)
            {
                Module.RegisterEvent(EventType.Warning, "Ошибка авторизации", ex.Message, ex.InnerException);
                message = ex.Message;
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(EventType.Warning, "Ошибка авторизации - непредвиденная", ex.Message, ex.InnerException);
                message = "Неожиданная ошибка во время авторизации. Попробуйте еще раз или обратитесь в техническую поддержку.";
            }

            if (!ModelState.IsValid || !string.IsNullOrEmpty(message)) this.RegisterEventInvalidModel("Форма авторизации JSON", ignoreParamsKeys: new List<string>() { nameof(model.pass) });

            return this.ReturnJson(success, message, new
            {
                authorized = !AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest,
                admin = this.Module.CheckPermission(ModulesConstants.PermissionManage)
            });
        }

        public ActionResult logout()
        {
            AppCore.GetUserContextManager().DestroyUserContext(AppCore.GetUserContextManager().GetCurrentUserContext());
            AppCore.Get<Binding.Providers.SessionBinder>().ClearUserContextFromRequest();
            AppCore.GetUserContextManager().ClearCurrentUserContext();
            return Redirect("/");
        }

        public ActionResult logoutJson()
        {
            var success = false;
            var message = "";

            try
            {
                AppCore.GetUserContextManager().DestroyUserContext(AppCore.GetUserContextManager().GetCurrentUserContext());
                AppCore.Get<Binding.Providers.SessionBinder>().ClearUserContextFromRequest();
                AppCore.GetUserContextManager().ClearCurrentUserContext();
                success = true;
                message = "Выход прошел успешно.";
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return this.ReturnJson(success, message, Module.GetRememberedUserContextRequestedAddressWhenRedirectedToAuthorization());
        }

        [ModuleAction("restore")]
        public ActionResult PasswordRestore()
        {
            if (Module.IsNeededAnyUserToRegister()) return Redirect<Register.ModuleRegister, Register.ModuleRegisterController>(x => x.Register());

            if (!AppCore.GetUserContextManager().GetCurrentUserContext().IsGuest) return RedirectToAction(nameof(Login));
            return display("PasswordRestore.cshtml", new Model.PasswordRestore());
        }

        [ModuleAction("restore2")]
        public JsonResult PasswordRestoreSend(Model.PasswordRestore model = null)
        {
            var answer = JsonAnswer<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.email) && !string.IsNullOrEmpty(model.phone)) ModelState.AddModelError(nameof(model.EmailOrPhone), "Следует указать либо адрес электронной почты, либо номер телефона.");
                }

                var isPhone = !string.IsNullOrEmpty(model.phone);

                if (ModelState.IsValid)
                {
                    if (!isPhone)
                    {
                        if (!AppCore.WebConfig.userAuthorizeAllowed.In(eUserAuthorizeAllowed.EmailAndPhone, eUserAuthorizeAllowed.OnlyEmail)) ModelState.AddModelError(nameof(model.email), "К сожалению, в данный момент авторизация через адрес электронной почты отключена, восстановление пароля таким способом невозможно.");
                    }
                    else if (isPhone)
                    {
                        if (!AppCore.WebConfig.userAuthorizeAllowed.In(eUserAuthorizeAllowed.EmailAndPhone, eUserAuthorizeAllowed.OnlyPhone)) ModelState.AddModelError(nameof(model.phone), "К сожалению, в данный момент авторизация через номер телефона отключена, восстановление пароля таким образом невозможно.");
                    }
                }

                if (ModelState.IsValid)
                {
                    using (var db = Module.CreateUnitOfWork())
                    {
                        var sql = from p in db.Users
                                  where (!isPhone && p.email.Length > 0 && p.email.ToLower() == model.email.ToLower()) || (isPhone && p.phone.Length > 0 && p.phone.ToLower() == model.phone.ToLower())
                                  select p;

                        var user = sql.FirstOrDefault();

                        if (user == null)
                        {
                            if (!isPhone)
                                throw new BehaviourException("Указанный Email-адрес не найден на сайте!");
                            else
                                throw new BehaviourException("Указанный номер телефона не найден на сайте!");
                        }
                        else
                        {
                            var codeType = !isPhone ? "email" : "phone";

                            using (var scope = db.CreateScope())
                            {
                                if (!isPhone)
                                {
                                    var code = DateTime.Now.Microtime().MD5();
                                    db.PasswordRemember.Add(new PasswordRemember() { user_id = user.IdUser, code = code });

                                    AppCore.Get<EmailService>().SendMailFromSite(
                                        user.Caption,
                                        user.email,
                                        "Восстановление пароля на сайте",
                                        this.ViewString("PasswordRestoreNotificationEmail.cshtml", new Design.Model.PasswordRestoreSend() { User = user, Code = code, CodeType = codeType }),
                                        ContentType.Html
                                    );
                                }
                                else
                                {
                                    var code = OnUtils.Utils.StringsHelper.GenerateRandomString("0123456789", 4);
                                    db.PasswordRemember.Add(new PasswordRemember() { user_id = user.IdUser, code = code });

                                    AppCore.Get<MessagingSMS.SMSService>().SendMessage(user.phone, "Код восстановления пароля: " + code);
                                }

                                db.SaveChanges();
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
            }
            catch (BehaviourException ex)
            {
                Module.RegisterEvent(EventType.Info, "Ошибка восстановления пароля", ex.Message, ex.InnerException);
                answer.FromFail(ex.Message);
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(EventType.Error, "Ошибка восстановления пароля", "Необработанная ошибка", ex);
                answer.FromFail("Возникла ошибка во время восстановления пароля");
            }

            if (!ModelState.IsValid) this.RegisterEventInvalidModel("Форма восстановления пароля - шаг 1");

            return ReturnJson(answer);
        }

        [ModuleAction("restore3")]
        public ActionResult PasswordRestoreVerify(string Code = null, string CodeType = null)
        {
            if (Module.IsNeededAnyUserToRegister()) return Redirect<Register.ModuleRegister, Register.ModuleRegisterController>(x => x.Register());

            return View("PasswordRestoreVerify.cshtml", new Design.Model.PasswordRestoreVerify() { Code = Code?.Truncate(0, 32), CodeType = CodeType?.Truncate(0, 6) });
        }

        [ModuleAction("restore4")]
        public ActionResult PasswordRestoreSave(Model.PasswordRestoreSave model = null)
        {
            var answer = JsonAnswer<string>();
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = Module.CreateUnitOfWork())
                    {
                        var res = (from p in db.PasswordRemember
                                   join u in db.Users on p.user_id equals u.IdUser
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
                                db.PasswordRemember.Delete(res.Password);

                                res.User.password = UsersExtensions.hashPassword(model.Password);
                                res.User.salt = salt;

                                db.SaveChanges();

                                scope.Complete();
                            }

                            answer.FromSuccess("Новый пароль был сохранен.");
                        }
                    }
                }
            }
            catch (BehaviourException ex)
            {
                Module.RegisterEvent(EventType.Info, "Ошибка восстановления пароля при сохранении", ex.Message, ex.InnerException);
                answer.FromFail(ex.Message);
            }
            catch (Exception ex)
            {
                Module.RegisterEvent(EventType.Error, "Ошибка восстановления пароля при сохранении", "Необработанная ошибка", ex);
                answer.FromFail("Возникла ошибка во время сохранения пароля");
            }

            if (!ModelState.IsValid) this.RegisterEventInvalidModel("Форма восстановления пароля - шаг 2");

            return ReturnJson(answer);
        }
    }
}