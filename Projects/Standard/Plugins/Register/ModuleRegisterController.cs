using OnUtils.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace OnWeb.Plugins.Register
{
    using Core.DB;
    using Core.Types;
    using Core.Exceptions;
    using CoreBind.Modules;

    public class ModuleRegisterController : ModuleControllerUser<ModuleRegister>
    {
        [ModuleAction("index")]
        public override ActionResult Index()
        {
            return Register();
        }

        [ModuleAction("register")]
        public virtual ActionResult Register()
        {
            var d = AppCore.Get<UsersManagement.ModuleUsersManagement>().GetUsersByRolePermission<UsersManagement.ModuleUsersManagement>(UsersManagement.ModuleUsersManagement.PermissionReceiveRegisterModeratorNotifications);

            return display("Register.cshtml", new Model.Register());
        }

        [ModuleAction("reg2")]
        public JsonResult RegisterSave(Model.Register model)
        {
            var answer = JsonAnswer<User>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new UnitOfWork<User>())
                    {
                        if (!string.IsNullOrEmpty(model.email))
                        {
                            if (db.Repo1.Where(x => x.email.ToLower() == model.email.ToLower()).Count() > 0) ModelState.AddModelError(nameof(model.email), "Этот email-адрес уже используется!");
                        }

                        if (!string.IsNullOrEmpty(model.phone))
                        {
                            var phone = PhoneBuilder.ParseString(model.phone);
                            if (!phone.IsCorrect) ModelState.AddModelError(nameof(model.phone), phone.Error);
                            else
                            {
                                model.phone = phone.ParsedPhoneNumber;
                                if (db.Repo1.Where(x => x.phone.ToLower() == model.phone.ToLower()).Count() > 0) ModelState.AddModelError(nameof(model.phone), "Этот номер телефона уже используется!");
                            }
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    var preparedData = new Model.PreparedForRegister();
                    var symbols = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    var salt = "";

                    var rand = new Random();
                    for (int i = 0; i < 5; i++)
                    {
                        var index = rand.Next(0, symbols.Length - 1);
                        salt = salt + symbols[index];
                    }

                    preparedData.salt = salt;

                    var regMode = (RegisterMode)AppCore.Config.Get("register_mode", 0);
                    switch (regMode)
                    {
                        case RegisterMode.SelfConfirmation:
                            preparedData.State = Core.DB.UserState.RegisterNeedConfirmation;
                            break;

                        case RegisterMode.Immediately:
                            preparedData.State = Core.DB.UserState.Active;
                            break;

                        case RegisterMode.ManualCheck:
                            preparedData.State = Core.DB.UserState.RegisterWaitForModerate;
                            break;
                    }

                    preparedData.email = model.email;
                    preparedData.phone = model.phone;

                    preparedData.IP_reg = Request.ServerVariables["REMOTE_ADDR"];
                    preparedData.password = model.password;

                    preparedData.name = model.name;

                    preparedData.DateReg = DateTime.Now;
                    preparedData.Fields.CopyValuesFrom(model.Fields);

                    preparedData.Superuser = 0;

                    var isSuperuserNeeded = AppCore.GetModulesManager().GetModule<Auth.ModuleAuth>()?.IsSuperuserNeeded() ?? false;
                    if (isSuperuserNeeded)
                    {
                        preparedData.Superuser = 1;
                        preparedData.State = Core.DB.UserState.Active;
                    }

                    var result = Module.RegisterUser(preparedData);
                    if (isSuperuserNeeded && result.Success) result.Message += " Учетная запись была помечена как \"Суперпользователь\" и немедленно активирована.";
                    answer = result;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("RegisterSave: {0}", ex.Message);
                answer.FromException(ex);
            }

            return ReturnJson(answer);
        }

        [ModuleAction("confirm")]
        public ActionResult Confirm(int idUser = 0, string confirmationCode = null)
        {
            //todo перенести из php.

            if (idUser <= 0) throw new ErrorCodeException(HttpStatusCode.BadRequest, "Неправильно указан пользователь.");
            if (string.IsNullOrEmpty(confirmationCode)) throw new ErrorCodeException(HttpStatusCode.BadRequest, "Не указан код подтверждения.");
            if (confirmationCode.Length >= 60) throw new ErrorCodeException(HttpStatusCode.BadRequest, "Некорректный код подтверждения.");

            try
            {
                var message = "";
                using (var db = new CoreContext())
                {
                    var data = db.Users.FirstOrDefault(x => x.id == idUser);
                    if (data == null)
                    {
                        message = "Пользователь с указанным идентификатором не найден.";
                    }
                    else
                    {
                        if (data.State == Core.DB.UserState.Active)
                        {
                            message = "Учетная запись уже активирована.";
                        }
                        else if (data.State != Core.DB.UserState.RegisterNeedConfirmation)
                        {
                            message = "Учетная запись не находится в состоянии ожидания подтверждения.";
                        }
                        else
                        {
                            if (data.StateConfirmation != confirmationCode)
                            {
                                message = "Код подтверждения не совпадает.";
                            }
                            else
                            {
                                data.State = Core.DB.UserState.Active;
                                data.StateConfirmation = "";
                                db.SaveChanges();
                                message = "Ваша учетная запись была активирована. Теперь вы можете зайти на сайт, используя реквизиты, указанные при регистрации.";
                            }
                        }
                    }
                }

                return View("Message.cshtml", new Design.Model.Message() { Subject = "Подтверждение после регистрации", Body = message });
            }
            catch (Exception ex)
            {
                this.RegisterEventWithCode(HttpStatusCode.InternalServerError, "Неизвестная ошибка при активации пользователя.", null, ex);
                throw;
            }
        }
    }
}
