using OnUtils.Data;
using OnUtils.Data.Validation;
using OnUtils.Utils;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnWeb.Plugins.Register
{
    using Core.DB;
    using Core.Journaling;
    using Core.Modules;
    using Core.Types;
    using CoreBind.Types;
    using MessagingEmail;
    using UsersManagement;

    [ModuleCore("Регистрация", DefaultUrlName = "Register")]
    public class ModuleRegister : ModuleCore<ModuleRegister>
    {
        protected ModelStateDictionary ValidateModel(object model)
        {
            ControllerContext controllerContext = null;
            if (HttpContext.Current != null)
            {
                var controller = HttpContext.Current.Items["RequestContextController"] as Controller;
                controllerContext = controller.ControllerContext;
            }

            if (controllerContext == null)
                controllerContext = new ControllerContext();

            if (model == null) throw new ArgumentNullException(nameof(model));

            var method_CreateSubPropertyName = typeof(DefaultModelBinder).GetMethod("CreateSubPropertyName", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (method_CreateSubPropertyName == null) throw new MissingMethodException(nameof(DefaultModelBinder), "CreateSubPropertyName");

            var metadataForType = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType());
            var modelState = new ModelStateDictionary();
            foreach (var current in ModelValidator.GetModelValidator(metadataForType, controllerContext).Validate(null))
            {
                var key = method_CreateSubPropertyName.Invoke(null, new object[] { string.Empty, current.MemberName }) as string;
                modelState.AddModelError(key, current.Message);
            }

            return modelState;
        }

        public ResultWData<User> RegisterUser(Model.PreparedForRegister data)
        {
            var answer = new ResultWData<User>();

            try
            {
                using (var db = new UnitOfWork<User>())
                {
                    var hasEmail = false;
                    var hasPhone = false;

                    var validationResult = ValidateModel(data);
                    if (!validationResult.ContainsKey(nameof(data.email)) && !string.IsNullOrEmpty(data.email))
                    {
                        data.email = data.email.ToLower();
                        hasEmail = true;
                        if (db.Repo1.Where(x => x.email == data.email).Count() > 0) validationResult.AddModelError(nameof(data.email), "Пользователь с таким адресом электронной почты уже существует!");
                    }
                    if (!validationResult.ContainsKey(nameof(data.phone)) && !string.IsNullOrEmpty(data.phone))
                    {
                        hasPhone = true;
                        if (db.Repo1.Where(x => x.phone == data.phone).Count() > 0) validationResult.AddModelError(nameof(data.phone), "Пользователь с таким номером телефона уже существует!");
                    }

                    if (validationResult.IsValid)
                    {
                        var salt = StringsHelper.GenerateRandomString("abcdefghijklmnoprstuvxyzABCDEFGHIKLMNOPRSTUVXYZ0123456789", 5);
                        var stateConfirmation = DateTime.Now.Ticks.ToString().MD5();

                        var regMode = AppCore.Config.register_mode;

                        if (data.State.HasValue)
                        {
                            switch (data.State)
                            {
                                case Core.DB.UserState.Active:
                                    regMode = RegisterMode.Immediately;
                                    break;

                                case Core.DB.UserState.RegisterWaitForModerate:
                                    regMode = RegisterMode.ManualCheck;
                                    break;

                                case Core.DB.UserState.RegisterNeedConfirmation:
                                    regMode = RegisterMode.SelfConfirmation;
                                    break;

                                default:
                                    throw new Exception("При регистрации пользователя нельзя указывать данное состояние учетной записи.");
                            }
                        }
                        else
                        {
                            switch (regMode)
                            {
                                case RegisterMode.Immediately:
                                    data.State = Core.DB.UserState.Active;
                                    break;

                                case RegisterMode.SelfConfirmation:
                                    data.State = Core.DB.UserState.RegisterNeedConfirmation;
                                    break;

                                case RegisterMode.ManualCheck:
                                    data.State = Core.DB.UserState.RegisterWaitForModerate;
                                    break;
                            }
                        }

                        var CommentAdmin = "";

                        var query = new User()
                        {
                            password = UsersExtensions.hashPassword(data.password),
                            salt = salt,
                            email = data.email?.ToLower(),
                            State = data.State.Value,
                            CommentAdmin = CommentAdmin,
                            name = !string.IsNullOrEmpty(data.name) ? data.name : "",
                            phone = data.phone,
                            about = data.about,
                            Comment = data.Comment,
                            UniqueKey = data.UniqueKey,
                            DateReg = data.DateReg.Timestamp(),
                            IP_reg = data.IP_reg,
                            Superuser = data.Superuser,
                            StateConfirmation = regMode == RegisterMode.SelfConfirmation ? stateConfirmation : string.Empty,
                        };

                        query.Fields.CopyValuesFrom(data.Fields);

                        using (var scope = db.CreateScope())
                        {
                            db.Repo1.Add(query);
                            db.SaveChanges();

                            var credentitals = string.Join(" или ", new string[] { hasEmail ? "адрес электронной почты" : string.Empty, hasPhone ? "номер телефона" : string.Empty }.Where(x => !string.IsNullOrEmpty(x)));

                            if (regMode == RegisterMode.Immediately)
                            {
                                if (hasEmail)
                                    AppCore.Get<IEmailService>().SendMailFromSite(
                                        data.name,
                                        data.email,
                                        "Регистрация на сайте",
                                        Core.WebUtils.RazorRenderHelper.RenderView(this, "RegisterNotificationEmailImmediately.cshtml", query)
                                    );

                                if (hasPhone)
                                    AppCore.Get<MessagingSMS.IService>()?.SendMessage(data.phone, "Регистрация на сайте прошла успешно.");

                                answer.FromSuccess($"Вы успешно зарегистрировались на сайте и можете зайти, используя {credentitals}.");
                            }
                            else if (regMode == RegisterMode.SelfConfirmation)
                            {
                                if (hasEmail)
                                    AppCore.Get<IEmailService>().SendMailFromSite(
                                        data.name,
                                        data.email,
                                        "Регистрация на сайте",
                                        Core.WebUtils.RazorRenderHelper.RenderView(this, "RegisterNotificationEmailConfirm.cshtml", new Model.RegisterNotificationConfirm() { Data = query, ConfirmationCode = query.StateConfirmation })
                                    );

                                answer.FromSuccess("Вы успешно зарегистрировались на сайте. В течение определенного времени на Ваш электронный адрес, указанный при регистрации, придет письмо с указаниями по дальнейшим действиям, необходимым для завершения регистрации.");
                            }
                            else if (regMode == RegisterMode.ManualCheck)
                            {
                                if (hasEmail)
                                    AppCore.Get<IEmailService>().SendMailFromSite(
                                        data.name,
                                        data.email,
                                        "Регистрация на сайте",
                                        Core.WebUtils.RazorRenderHelper.RenderView(this, "RegisterNotificationEmailModerate.cshtml", query)
                                    );

                                answer.FromSuccess($"Заявка на регистрацию отправлена. Администратор рассмотрит Вашу заявку и примет решение, после чего Вы получите уведомление на указанный {credentitals}.");

                                var usersToNotify = AppCore.Get<ModuleUsersManagement>().GetUsersByRolePermission<ModuleUsersManagement>(ModuleUsersManagement.PermissionReceiveRegisterModeratorNotifications);
                                if (usersToNotify.Count > 0)
                                {
                                    var mailAdmin = Core.WebUtils.RazorRenderHelper.RenderView(this, "RegisterNotificationEmailAdmin.cshtml", query);
                                    usersToNotify.
                                        Where(x => !string.IsNullOrEmpty(x.email)).
                                        ForEach(x => AppCore.Get<IEmailService>().SendMailFromSite(x.email, x.email, "Новая заявка на регистрацию", mailAdmin));
                                }
                            }

                            answer.Data = query;

                            if (answer.Success) scope.Commit();
                        }
                    }
                    else
                    {
                        var errorMessages = new System.Collections.ObjectModel.Collection<string>();
                        validationResult.ForEach(x => x.Value.Errors.ForEach(error => errorMessages.Add(error.ErrorMessage)));
                        answer.FromFail("Возникли ошибки во время проверки данных:\r\n - " + string.Join(";\r\n - ", errorMessages) + ".");
                    }
                }
            }
            catch (EntityValidationException ex)
            {
                Debug.Logs("RegisterUser1: {0}", ex.CreateComplexMessage());
                answer.FromFail(ex.CreateComplexMessage());
            }
            catch (HandledException ex)
            {
                Debug.Logs("RegisterUser2: {0}", ex.Message);
                this.RegisterEvent(EventType.Warning, "Регистрация пользователя", "Данные: " + Newtonsoft.Json.JsonConvert.SerializeObject(data), ex);
                answer.FromFail("Регистрация прервана из-за ошибки: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Logs("RegisterUser2: {0}", ex.Message);
                this.RegisterEvent(EventType.Error, "Регистрация пользователя - необработанная ошибка", "Данные: " + Newtonsoft.Json.JsonConvert.SerializeObject(data), ex);
                answer.FromFail("Регистрация прервана из-за непредвиденной ошибки");
            }

            return answer;
        }
    }
}
