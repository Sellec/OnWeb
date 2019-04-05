using OnUtils.Application.Modules;
using OnUtils.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Customer
{
    using Core.DB;
    using CoreBind.Modules;
    using FileManager;

    public class ModuleControllerCustomer : ModuleControllerUser<ModuleCustomer>
    {
        protected override void onDisplayModule(object model)
        {
            var tags = (from p in DB.UserEntity where p.IsTagged group p.Tag by p.Tag into gr orderby gr.Key select gr.Key).ToList();

            var externalParts = new Hashtable();
            //externalParts["/".this.getName()."/external/energy/counters"] = "Счетчики";

            this.assign("data", AppCore.GetUserContextManager().GetCurrentUserContext().GetData());

            this.assign("customer_tags", tags);
            this.assign("externalParts", externalParts);
            this.assign("is_customer", 1);

        }

        /*
         * Главная инфо профиля
         * */
        [ModuleAction(null, ModulesConstants.PermissionAccessUserString)]
        public ActionResult Index(string part = null)
        {
            var data = AppCore.GetUserContextManager().GetCurrentUserContext().GetData();
            return this.display("customerIndex.cshtml", data);
        }

        /*
         * Показывает окно редактирования личных данных.
         * */
        [ModuleAction("datas", ModulesConstants.PermissionAccessUserString)]
        public ActionResult profile()
        {
            return Index();

            var data = AppCore.GetUserContextManager().GetCurrentUserContext().GetData();
            return this.display("customerProfile.cshtml", data);
        }

        /*
         * Показывает окно редактирования личных данных на сокращенной форме, для подгрузки через Ajax.
         * */
        [ModuleAction("editLoad", ModulesConstants.PermissionAccessUserString)]
        public ActionResult ProfileEditAjax(int? IdUser = null)
        {
            if (!IdUser.HasValue) IdUser = AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser();

            using (var db = new UnitOfWork<User>())
            {
                var data = db.Repo1.Where(x => x.id == IdUser.Value).FirstOrDefault();
                if (data == null) throw new Exception("Указанный пользователь не найден.");
                else Module.CheckPermissionToEditOtherUser(IdUser.Value);

                var d = data.Fields;
                return this.display("customerProfileInner.cshtml", new Design.Model.Profile()
                {
                    User = data,
                    Edit = new Model.ProfileEdit(data)
                });
            }
        }

        /*
         * Сохранение личных данных.
         * */
        [ModuleAction("datas_save", ModulesConstants.PermissionAccessUserString)]
        public ActionResult profileSave([Bind(Prefix = nameof(Design.Model.Profile.Edit))] Model.ProfileEdit model)
        {
            var answer = JsonAnswer<User>();
            var prefix = nameof(Design.Model.Profile.Edit) + ".";

            try
            {
                //if (!this.IsReCaptchaValid && !AppCore.GetUserContextManager().GetCurrentUserContext().IsSuperuser) ModelState.AddModelError("ReCaptcha", "Вы точно не робот?");
                if (model == null) throw new Exception("Нет переданных данных.");

                using (var db = new UnitOfWork<User>())
                using (var trans = db.CreateScope())
                {
                    var data = db.Repo1.Where(x => x.id == model.ID).FirstOrDefault();
                    if (data == null) throw new Exception("Указанный пользователь не найден.");
                    else Module.CheckPermissionToEditOtherUser(model.ID);

                    if (ModelState.IsValid)
                    {
                        if (ModelState.Keys.Contains(prefix + nameof(model.email)))
                        {
                            model.email = model.email?.ToLower();
                            data.email = data.email?.ToLower();
                            if (data.email != model.email)
                            {
                                var others = db.Repo1.AsNoTracking().Where(x => x.email.ToLower() == model.email && x.id != data.id).Count();
                                if (others > 0) ModelState.AddModelError(prefix + nameof(model.email), "Такой email-адрес уже используется!");
                                else data.email = model.email;
                            }
                        }

                        if (ModelState.Keys.Contains(prefix + nameof(model.phone)))
                        {
                            model.phone = model.phone?.ToLower();
                            data.phone = data.phone?.ToLower();
                            if (data.phone != model.phone)
                            {
                                if (string.IsNullOrEmpty(model.phone)) model.phone = null;
                                else
                                {
                                    var others = db.Repo1.AsNoTracking().Where(x => x.phone.ToLower() == model.phone && x.id != data.id).Count();
                                    if (others > 0) ModelState.AddModelError(prefix + nameof(model.phone), "Такой номер телефона уже используется!");
                                    else
                                    {
                                        var phone = PhoneBuilder.ParseString(model.phone);
                                        if (!phone.IsCorrect)
                                            ModelState.AddModelError(prefix + nameof(model.phone), phone.Error);
                                        else
                                            data.phone = phone.ParsedPhoneNumber;
                                    }
                                }
                            }
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        if (ModelState.Keys.Contains(prefix + nameof(model.name))) data.name = model.name;
                        if (ModelState.Keys.Contains(prefix + nameof(model.IdPhoto)))
                        {
                            if (model.IdPhoto.HasValue) AppCore.Get<FileManager>().UpdateExpiration(model.IdPhoto.Value, null);
                            data.IdPhoto = model.IdPhoto;
                        }
                        if (ModelState.Keys.Contains(prefix + nameof(model.Comment))) data.Comment = model.Comment;

                        data.Fields.CopyValuesFrom(model.Fields, fdata => ModelState.Keys.Contains(prefix + "Fields.fieldValue_" + fdata.IdField));
                        data.DateChange = DateTime.Now.Timestamp();

                        db.SaveChanges();

                        AppCore.GetUserContextManager().CreateUserContext(AppCore.GetUserContextManager().GetCurrentUserContext().GetIdUser());

                        trans.Commit();

                        answer.Data = data;

                        answer.FromSuccess("Сохранение данных прошло успешно!");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ProfileSave: {0}", ex);
                answer.FromException(ex);
            }

            return ReturnJson(answer);
        }

        /*
         * Форма смены пароля
         * */
        [ModuleAction("pchange", ModulesConstants.PermissionAccessUserString)]
        public ActionResult passwordChange()
        {
            return this.display("customerPassword.cshtml", AppCore.GetUserContextManager().GetCurrentUserContext().GetData());
        }

        /*
         * Смена пароля
         * */
        [ModuleAction("pchange2", ModulesConstants.PermissionAccessUserString)]
        public ActionResult passwordChange2(Model.UserPasschange model)
        {
            var success = false;
            var result = new List<string>();

            try
            {
                if (!ModelState.IsValid)
                    result.AddRange(ModelState.Values.Where(y => y.Errors.Count > 0).
                                    SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList());

                var data_ = AppCore.GetUserContextManager().GetCurrentUserContext().GetData();

                if (result.Count == 0)
                {
                    if (!ModelState.Keys.Contains("passwordOld")) result.Add("Не указан текущий пароль.");
                    else if (UsersExtensions.hashPassword(model.passwordOld) != data_.password) result.Add("Неправильный пароль.");

                    if (!ModelState.Keys.Contains("passwordNew")) result.Add("Не указан новый пароль.");
                }

                if (result.Count == 0)
                {
                    data_.password = UsersExtensions.hashPassword(model.passwordNew);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                result.Add(ex.Message);
                success = false;
            }
            if (success) DB.SaveChanges();// else DB.RevertChanges();

            return this.ReturnJson(success, result.Count > 0 ? " - " + string.Join("\r\n - ", result) : "");
        }

        //public function tag_show(tag = null)
        //{
        //    if (tag == null) throw new Exception("Не указан тег!");
        //    else if (!self::isOneStringTextOnly(tag) || DataManager.check(tag)) throw new Exception("Неправильно указан тег!");
        //    else 
        //    {
        //        text = DataManager.prepare(tag);

        //        //Узнаем список тегов пользователя.
        //        tags = this.getUserManager().getUserEntities(text);

        //        //Передаем список тегов по ссылке в шаблон.
        //        this.assignRef("tag", tag);
        //        this.assignRef("tags", tags);

        //        //Показываем шаблон. Шаблон вызовет для КАЖДОГО тега метод OnShowDataInCustomer. 
        //        //Надо в каждом типе тега в методе OnShowDataInCustomer выводить html-код для показа в ЛК.
        //        this.display(this.getUserManager().isAuthorized() ? "customer_tags_show.cshtml" : "customer_tags_show_unregistered.cshtml");
        //    }
        //}

        //public function tag_delete(IdEntity = null)
        //{
        //    success = false;
        //    result = "";

        //    if (!self::isInt(IdEntity)) result = "Некорректно указан тег!";
        //    else 
        //    {
        //        tag = this.Main.EntitiesManager.getEntity(IdEntity);
        //        if (tag == null) result = this.Main.EntitiesManager.Error;
        //        else 
        //        {
        //            if (this.Main.EntitiesManager.deleteEntity(tag)) success = true;
        //            else result = this.Main.EntitiesManager.Error;                 
        //        }
        //    }

        //    this.ReturnJson(success, result);
        //}

        ///*
        //Показывает окно смены пароля.
        //*/
        //public function external(NameModule = null, Action = null)
        //{
        //    includeCode = "";

        //    if (NameModule != null && is_string(NameModule) && strlen(NameModule) > 0)
        //    {
        //        modname = this.Main.getConfigModule(NameModule);
        //        load = ModulesManager::loadModule(modname);
        //        if (load !== true) includeCode = "Ошибка загрузки модуля "NameModule" ("modname"): load!";
        //        else 
        //        {
        //            /*ob_start();

        //            args = is_array(args) ? args : array_slice(func_get_args(), 2);
        //            result = this.Main.startNoEcho(module, action, args, true);
        //            if (!result) result = this.Main.Error;

        //            code_to_show = ob_get_contents();
        //            ob_end_clean();

        //            if (result !== true) code_to_show = result;

        //            if (result !== true) includeCode = result;
        //            else includeCode = code;*/

        //            includeCode = "Нет доступных расширений.";
        //        }
        //    }

        //    this.assign("include_code", includeCode);
        //    this.display("customer_external.cshtml");
        //}    

        //function subscribes()
        //{
        //    sql = DataManager.executeQuery("SELECT * FROM users WHERE id="".this.getUserManager().getID().""");
        //    if (DataManager.getNumSelectedRecords(sql) == 0) throw new Exception("В базе данных не найдено вашей учетной записи!");

        //    listByEmail = listByRole = array();

        //    /**
        //    * Достаем все рассылки, в которых когда-либо участвовал пользователь.
        //    */
        //    sql = DataManager.executeQuery("
        //        SELECT Subscription.*, CASE WHEN IFNULL(SubscriptionEmail.id, 0) > 0 THEN 1 ELSE 0 END as IsEnabled
        //        FROM Subscription
        //        INNER JOIN SubscriptionHistory ON Subscription.id = SubscriptionHistory.subscr_id
        //        LEFT JOIN SubscriptionEmail ON Subscription.id = SubscriptionEmail.subscr_id AND SubscriptionEmail.email=SubscriptionHistory.email
        //        WHERE SubscriptionHistory.email="".this.getUserManager().getEmail().""
        //    ");
        //    while (res = DataManager.getSelectedRecord(sql)) listByEmail[res["id"]] = res;

        //    /**
        //    * Достаем все рассылки, в которых любому пользователю разрешено подписываться.
        //    */
        //    sql = DataManager.executeQuery("
        //        SELECT Subscription.*, CASE WHEN IFNULL(SubscriptionEmail.id, 0) > 0 THEN 1 ELSE 0 END as IsEnabled
        //        FROM Subscription
        //        LEFT JOIN SubscriptionEmail ON Subscription.id = SubscriptionEmail.subscr_id AND SubscriptionEmail.email="".this.getUserManager().getEmail().""
        //        WHERE Subscription.AllowSubscribe <> 0
        //    ");
        //    while (res = DataManager.getSelectedRecord(sql)) listByEmail[res["id"]] = res;

        //    uasort(listByEmail, function (e1, e2) { return strcmp(e1["name"], e2["name"]); });

        //    sql = DataManager.executeQuery("
        //        SELECT Subscription.*, Role.IdRole, Role.NameRole 
        //        FROM Subscription
        //        INNER JOIN SubscriptionRole ON Subscription.id = SubscriptionRole.IdSubscription
        //        INNER JOIN Role ON Role.IdRole = SubscriptionRole.IdRole
        //        INNER JOIN RoleUser ON Role.IdRole = RoleUser.IdRole
        //        WHERE RoleUser.IdUser="".this.getUserManager().getID().""
        //    ");
        //    while (res = DataManager.getSelectedRecord(sql)) 
        //    {
        //        if (!isset(listByRole[res["id"]])) listByRole[res["id"]] = res;
        //        listByRole[res["id"]]["Roles"][res["IdRole"]] = res["NameRole"];
        //    }

        //    this.assign("listByEmail", listByEmail);
        //    this.assign("listByRole", listByRole);
        //    this.display("customer_subscriptions.cshtml");
        //}

        //function subscribesUnsubscribe(IdSubscription = null)
        //{
        //    success = false;
        //    message = "";

        //    try
        //    {
        //        sql = DataManager.executeQuery("SELECT * FROM users WHERE id="".this.getUserManager().getID().""");
        //        if (DataManager.getNumSelectedRecords(sql) == 0) throw new Exception("В базе данных не найдено вашей учетной записи!");

        //        if (!self::isInt(IdSubscription)) throw new Exception("Неправильно задан номер рассылки.");

        //        success = SubscriptionsManager::unsubscribeEmail(IdSubscription, this.getUserManager().getEmail());
        //        if (!success) message = SubscriptionsManager::getError();
        //    }
        //    catch(Exception ex)
        //    {
        //        success = false;
        //        message = ex.getMessage();
        //    }

        //    this.ReturnJson(success, message);
        //}

        //function subscribesSubscribe(IdSubscription = null)
        //{
        //    success = false;
        //    message = "";

        //    try
        //    {
        //        sql = DataManager.executeQuery("SELECT * FROM users WHERE id="".this.getUserManager().getID().""");
        //        if (DataManager.getNumSelectedRecords(sql) == 0) throw new Exception("В базе данных не найдено вашей учетной записи!");

        //        if (!self::isInt(IdSubscription)) throw new Exception("Неправильно задан номер рассылки.");
        //        sql = DataManager.executeQuery("SELECT * FROM Subscription WHERE id="IdSubscription"");
        //        if (DataManager.getNumSelectedRecords(sql) == 0) throw new Exception("Указанная рассылка не существует!");

        //        data = DataManager.getSelectedRecord(sql);
        //        if (data["AllowSubscribe"] == 0) 
        //        {
        //            //throw new Exception("Самостоятельная подписка на эту рассылку невозможна.");

        //            sql = DataManager.executeQuery("SELECT * FROM SubscriptionHistory WHERE subscr_id="IdSubscription" AND email="".this.getUserManager().getEmail().""");
        //            if (DataManager.getNumSelectedRecords(sql) == 0) throw new Exception("Вы не можете подписаться на эту рассылку!");
        //        }

        //        success = SubscriptionsManager::subscribeEmail(IdSubscription, this.getUserManager().getEmail());
        //        if (!success) message = SubscriptionsManager::getError();
        //    }
        //    catch(Exception ex)
        //    {
        //        success = false;
        //        message = ex.getMessage();
        //    }

        //    this.ReturnJson(success, message);
        //}
    }
}
