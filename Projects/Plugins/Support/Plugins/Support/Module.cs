using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.Mvc;

using OnUtils.Data;
using OnUtils.Data.Validation;

namespace OnWeb.Plugins.Support
{
    using Types;

    [ModuleCore("Support", "Поддержка")]
    public class Module: ModuleCore2<Configuration.CoreContext>
    {
        internal override void InitModuleImmediately(List<ModuleCoreCandidate> candidatesTypes)
        {
            base.InitModuleImmediately(candidatesTypes);
            this.AutoRegister();
        }

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

        public ResultWData<DB.User> RegisterUser(Model.PreparedForRegister data)
        {
            var answer = new ResultWData<DB.User>();

            try
            {
                using (var db = new UnitOfWork<TraceWeb.DB.User>())
                {
                    var validationResult = ValidateModel(data);
                    if (!validationResult.ContainsKey(nameof(data.email)) && !string.IsNullOrEmpty(data.email))
                    {
                        data.email = data.email.ToLower();
                        if (db.Repo1.Where(x => x.email == data.email).Count() > 0) validationResult.AddModelError(nameof(data.email), "Пользователь с таким Email-адресом уже существует!");
                    }
                    if (!validationResult.ContainsKey(nameof(data.phone)) && !string.IsNullOrEmpty(data.phone))
                    {
                        if (db.Repo1.Where(x => x.phone == data.phone).Count() > 0) validationResult.AddModelError(nameof(data.phone), "Пользователь с таким номером телефона уже существует!");
                    }

                    if (validationResult.IsValid)
                    {
                        var symbols = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                        var random = new Random();

                        var salt = new string(Enumerable.Repeat(symbols, 5).Select(s => s[random.Next(s.Length)]).ToArray());
                        var stateConfirmation = new string(Enumerable.Repeat(symbols, 32).Select(s => s[random.Next(s.Length)]).ToArray());

                        var regMode = ApplicationCore.Instance.Config.Get("register_mode", 0);
                        if (data.State.HasValue)
                            switch (data.State.Value)
                            {
                                case TraceWeb.DB.UserState.Active:
                                    regMode = 0;
                                    break;

                                case TraceWeb.DB.UserState.RegisterWaitForModerate:
                                    regMode = 2;
                                    break;

                                case TraceWeb.DB.UserState.RegisterNeedConfirmation:
                                    regMode = 1;
                                    break;

                                default:
                                    throw new Exception("При регистрации пользователя нельзя указывать данное состояние учетной записи.");
                            }

                        var CommentAdmin = "";

                        var query = new TraceWeb.DB.User()
                        {
                            password = UserManager.hashPassword(data.password),
                            salt = salt,
                            email = data.email?.ToLower(),
                            State = (DB.UserState)(short)regMode,
                            CommentAdmin = CommentAdmin,
                            name = !string.IsNullOrEmpty(data.name) ? data.name : "",
                            phone = data.phone,
                            about = data.about,
                            Comment = data.Comment,
                            UniqueKey = data.UniqueKey,
                            DateReg = data.DateReg.Timestamp(),
                            IP_reg = data.IP_reg,
                            Superuser = data.Superuser,
                            StateConfirmation = regMode == 2 ? stateConfirmation : string.Empty,
                            //IP_reg = Request.ServerVariables["REMOTE_ADDR"]
                        };

                        query.Fields.CopyValuesFrom(data.Fields);

                        using (var scope = db.CreateScope())
                        {
                            db.Repo1.Add(query);
                            db.SaveChanges();

                            var id = -1;
                            var can = false;

                            {
                                id = query.id;

                                //$exts = $this->getUserManager()->getExtensionsWithMethod('register');
                                //$added = array();
                                can = true;

                                //      foreach ($exts as $k =>$v)
                                //{
                                // $res = $v->register(array(
                                //              'login'         =>  data.login"],
                                //              'pass'          =>  data.password"],
                                //              'email'         =>  data.email"],

                                //          ));
                                //          if ($res) $added[$k] = $v;
                                // else 
                                // {
                                //              foreach ( $added as $k2 =>$v2) if (method_exists($v2, 'userDelete')) $v2->userDelete(data.reg_login"]);
                                //  $can = false;
                                //              break;
                                //          }
                                //      }
                            }

                            if (can)
                            {
                                var new_id = id;// = DataManager.getInsertedID();
                                if (id > 0)
                                {
                                    if (regMode == 0) answer.FromSuccess("Вы успешно зарегистрировались на сайте и можете зайти, используя логин и пароль, указанные при регистрации.");
                                    else if (regMode == 1) answer.FromSuccess("Вы успешно зарегистрировались на сайте. В течение определенного времени на Ваш электронный адрес, указанный при регистрации, придет письмо с указаниями по дальнейшим действиям, необходимым для завершения регистрации.");
                                    else if (regMode == 2) answer.FromSuccess("Заявка на регистрацию отправлена. Администратор рассмотрит Вашу заявку и примет решение, после чего Вы получите уведомление на указанный адрес электронной почты.");

                                    //this.assign("message", string.Join("\r\n - ", result.Values));
                                    //this.assign("login", data.email);

                                    //Module.DB.RoleUser.
                                    //    AddOrUpdate(new TraceWeb.DB.RoleUser()
                                    //    {
                                    //        IdRole = 1,
                                    //        IdUser = new_id,
                                    //        IdUserChange = 0,
                                    //        DateChange = DateTime.Now.Timestamp()
                                    //    });
                                    //Module.DB.SaveChanges();

                                    //if (regMode != 2)
                                    //    Messaging.Manager.Email.sendMailFromSite(
                                    //        data.email"],
                                    //        data.email"],
                                    //        "Регистрация на сайте",
                                    //        this.displayToVar("register_mail.cshtml")
                                    //    );

                                    //if (regMode == 2)
                                    //{
                                    //    this.assign("id", id);
                                    //    this.assign("login", data.email"]);
                                    //    this.assign("comment", data.comment"]);
                                    //    Messaging.Manager.Email.sendMailToAdmin("Новая заявка на регистрацию", this.displayToVar("register_mail_admin.cshtml"));
                                    //    Messaging.Manager.Email.sendMailSubscription(1, "Новая заявка на регистрацию", this.displayToVar("register_mail_admin.cshtml"));
                                    //    Messaging.Manager.Email.sendMailFromSite(data.email"], data.email"], "Регистрация на сайте", this.displayToVar("register_mail2.cshtml"));
                                    //}

                                    //success = "<br>";
                                    answer.Data = query;
                                }
                            }
                            else answer.FromFail("Ошибка во время регистрации пользователя. Обратитесь к администратору сайта.");

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
            catch (Exception ex)
            {
                Debug.Logs("RegisterUser2: {0}", ex.Message);
                answer.FromException(ex);
            }

            return answer;
        }

        ///public ConcurrentDictionary<string, >
    }
}
