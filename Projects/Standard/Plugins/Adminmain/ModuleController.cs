using System;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Adminmain
{
    using AdminForModules.Menu;
    using Core.Configuration;
    using Core.Modules;
    using CoreBind.Modules;
    using CoreBind.Routing;

    /// <summary>
    /// Представляет контроллер для панели управления.
    /// </summary>
    public class ModuleController : ModuleControllerAdmin<Module, DataContext>
    {
        [MenuAction("Настройки", "info", Module.PERM_CONFIGMAIN)]
        public virtual ActionResult MainSettings()
        {
            var model = new Model.AdminMainModelInfoPage(AppCore.Config)
            {
                ModulesList = (from p in AppCore.GetModulesManager().GetModules().OfType<ModuleCore>()
                               where p.ControllerTypes != null
                               orderby p.Caption
                               select new SelectListItem()
                               {
                                   Value = p.ID.ToString(),
                                   Text = p.Caption,
                                   Selected = AppCore.Config.IdModuleDefault == p.ID
                               }).ToList()
            };

            //var materials = AppCore.Get<ModulesManager>().GetModule<Materials.ModuleMaterials>();
            //if (materials != null)
            //    model.PagesList = (from p in materials.getPagesList()
            //                       orderby p.name ascending
            //                       select new SelectListItem()
            //                       {
            //                           Value = p.id.ToString(),
            //                           Text = p.name,
            //                           Selected = AppCore.Config.index_page == p.id
            //                       }).ToList();

            model.PagesList.Insert(0, new SelectListItem()
            {
                Value = "0",
                Text = "Не выбрано",
                Selected = AppCore.Config.index_page == 0
            });

            return this.display("admin_adminmain_info.tpl", model);
        }

        [ModuleAction("info_save", Module.PERM_CONFIGMAIN)]
        public virtual JsonResult MainSettingsSave(Model.AdminMainModelInfoPage model)
        {
            var result = JsonAnswer();

            try
            {
                if (ModelState.IsValid)
                {
                    var cfg = AppCore.GetModulesManager().GetModule<Plugins.CoreModule.CoreModule>().GetConfigurationManipulator().GetEditable<CoreConfiguration>();

                    cfg.IdModuleDefault = model.Configuration.IdModuleDefault;
                    cfg.index_page = model.Configuration.index_page;
                    cfg.SiteFullName = model.Configuration.SiteFullName;
                    cfg.helpform_email = model.Configuration.helpform_email;
                    cfg.register_mode = model.Configuration.register_mode;
                    cfg.site_reginfo = model.Configuration.site_reginfo;
                    cfg.site_loginfo = model.Configuration.site_loginfo;
                    cfg.help_info = model.Configuration.help_info;
                    cfg.site_descr = model.Configuration.site_descr;
                    cfg.site_keys = model.Configuration.site_keys;

                    cfg.reCaptchaSecretKey = model.Configuration.reCaptchaSecretKey;
                    cfg.reCaptchaSiteKey = model.Configuration.reCaptchaSiteKey;

                    cfg.userAuthorizeAllowed = model.Configuration.userAuthorizeAllowed;

                    switch (AppCore.GetModulesManager().GetModule<Plugins.CoreModule.CoreModule>().GetConfigurationManipulator().ApplyConfiguration(cfg))
                    {
                        case ApplyConfigurationResult.PermissionDenied:
                            result.Message = "Недостаточно прав для сохранения конфигурации системы.";
                            result.Success = false;
                            break;

                        case ApplyConfigurationResult.Success:
                        default:
                            System.Web.Routing.RouteTable.Routes.Where(x => x is RouteWithDefaults).Select(x => x as RouteWithDefaults).ForEach(x => x.UpdateDefaults());

                            result.Message = "Сохранено успешно!";
                            result.Success = true;
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return this.ReturnJson(result);
        }

        [MenuAction("Модули", "modules", Module.PERM_MODULES)]
        public ActionResult ModulesList()
        {
            var model = new Model.Modules()
            {
                //Unregistered = (from p in ModulesManager.getModulesCandidates()
                //                orderby p.Key.Info.ModuleCaption
                //                select new Model.ModuleUnregistered(p.Key) { Info = p.Value }).ToList(),
                Registered = (from p in AppCore.GetModulesManager().GetModules()
                              orderby p.Caption
                              select new Model.Module(p)).ToList()
            };

            return this.display("admin_adminmain_modules.cshtml", model);
        }

        //[ModuleAction("modedit", Module.PERM_MODULES)]
        //public ActionResult ModuleEdit(string ModuleName = null)
        //{
        //    if (string.IsNullOrEmpty(ModuleName)) throw new Exception("Не указано имя модуля для редактирования.");

        //    Model.ModuleUnregistered model = null;

        //    var module = ModulesManager.getModuleByNameBase(ModuleName);
        //    if (module == null) module = ModulesManager.getModuleByName(ModuleName);
        //    if (module != null) model = new Model.ModuleRegistered(module);

        //    if (module == null)
        //    {
        //        var moduleCandidate = (from p in ModulesManager.getModulesCandidates()
        //                               where p.Key.Info.ModuleBaseName == ModuleName
        //                               select new Model.ModuleUnregistered(p.Key) { Info = p.Value }).FirstOrDefault();
        //        if (moduleCandidate != null) model = moduleCandidate;
        //    }

        //    if (model == null)
        //    {
        //        if (ModulesManager.hasError()) throw new Exception(ModulesManager.getError());
        //        else throw new Exception(string.Format("Не получилось найти модуль с именем '{0}'", ModuleName));
        //    }

        //    return this.display("ModuleEdit.cshtml", model);
        //}

        //[ModuleAction("modsave", Module.PERM_MODULES)]
        //public JsonResult ModuleSave(string ModuleName = null, string alias = null)
        //{
        //    var success = false;
        //    var result = "";

        //    try
        //    {
        //        if (string.IsNullOrEmpty(ModuleName)) throw new Exception("Имя сохраняемого модуля не может быть пустым.");

        //        if (string.IsNullOrEmpty(alias)) alias = ModuleName.ToLower();
        //        if (string.IsNullOrEmpty(alias)) throw new Exception("Псевдоним модуля не может быть пустым.");

        //        Model.ModuleUnregistered model = null;

        //        var module = ModulesManager.getModuleByNameBase(ModuleName);
        //        if (module == null) module = ModulesManager.getModuleByName(ModuleName);
        //        if (module != null) model = new Model.ModuleRegistered(module);

        //        if (module == null)
        //        {
        //            var moduleCandidate = (from p in ModulesManager.getModulesCandidates()
        //                                   where p.Key.Info.ModuleBaseName == ModuleName
        //                                   select new Model.ModuleUnregistered(p.Key) { Info = p.Value }).FirstOrDefault();
        //            if (moduleCandidate != null) model = moduleCandidate;
        //        }

        //        if (model == null)
        //        {
        //            if (ModulesManager.hasError()) throw new Exception(ModulesManager.getError());
        //            else throw new Exception(string.Format("Не получилось найти модуль с именем '{0}'", ModuleName));
        //        }

        //        var moduleBaseName = model.NameBase;

        //        var config = DB.ConfigModules.Where(x => x.m_filename == moduleBaseName).FirstOrDefault();
        //        if (config == null)
        //        {
        //            config = new config_modules() { m_filename = moduleBaseName };
        //            DB.ConfigModules.Add(config);
        //        }

        //        config.m_name = alias;
        //        DB.SaveChanges();

        //        if (module != null) ModulesManager.unloadModule(module);
        //        ModulesManager.loadModules();

        //        success = true;
        //        result = "Настройки модуля сохранены, модуль перезагружен.";
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        result = ex.Message;
        //    }

        //    return ReturnJson(success, result);
        //}

        //[ModuleAction("moddelete", Module.PERM_MODULES)]
        //public JsonResult ModuleDelete(string ModuleName = null)
        //{
        //    var success = false;
        //    var result = "";

        //    try
        //    {
        //        if (string.IsNullOrEmpty(ModuleName)) throw new Exception("Не указано имя модуля для редактирования.");

        //        var module = ModulesManager.getModuleByNameBase(ModuleName);
        //        if (module == null) module = ModulesManager.getModuleByName(ModuleName);

        //        if (module == null) throw new Exception(string.Format("Не получилось найти модуль с именем '{0}' среди загруженных модулей. Среди кандидатов поиск не ведется - нечего удалять.", ModuleName));

        //        var moduleBaseName = module.getNameBase();

        //        var config = DB.ConfigModules.Where(x => x.m_filename == moduleBaseName).FirstOrDefault();
        //        if (config != null)
        //        {
        //            DB.ConfigModules.Delete(config);
        //            DB.SaveChanges();
        //        }

        //        ModulesManager.unloadModule(module);
        //        ModulesManager.loadModules();

        //        success = true;
        //        result = "Настройки модуля удалены и модуль выгружен.";
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        result = ex.Message;
        //    }

        //    return ReturnJson(success, result);
        //}

        //[ModuleAction(null, Module.PERM_MODULES)]
        //public JsonResult ModuleReload(string ModuleName = null)
        //{
        //    var success = false;
        //    var result = "";

        //    try
        //    {
        //        if (string.IsNullOrEmpty(ModuleName)) throw new Exception("Не указано имя модуля для перезагрузки.");

        //        var module = ModulesManager.getModuleByNameBase(ModuleName);
        //        if (module == null) module = ModulesManager.getModuleByName(ModuleName);

        //        if (module == null) throw new Exception(string.Format("Не получилось найти модуль с именем '{0}' среди загруженных модулей. Среди кандидатов поиск не ведется - нечего перезагружать.", ModuleName));

        //        ModulesManager.unloadModule(module);
        //        ModulesManager.loadModules();

        //        success = true;
        //        result = "Модуль перезагружен.";
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        result = ex.Message;
        //    }

        //    return ReturnJson(success, result);
        //}

        //[MenuAction("Редактируемое меню", "editablemenu", Module.PERM_EDITABLEMENU)]
        //public ActionResult EditableMenu()
        //{
        //    var model = DB.EditableMenu.OrderBy(x => x.id).ToList();

        //    return this.display("admin_adminmain_editablemenu.cshtml", model);
        //}

        //[ModuleAction("editablemenu_edit", Module.PERM_EDITABLEMENU)]
        //public ActionResult EditableMenuEdit(int IdMenu = 0)
        //{
        //    var menu = IdMenu == 0 ? new menus() : DB.EditableMenu.Where(x => x.id == IdMenu).FirstOrDefault();
        //    if (menu == null) throw new Exception(string.Format("Меню с id={0} не найдено.", IdMenu));

        //    return this.display("admin_adminmain_editablemenu_ae.cshtml", new Model.EditableMenu()
        //    {
        //        Menu = menu,
        //        Modules = (from p in ModulesManager.getModules() orderby p.getCaption() select p).ToList()
        //    });
        //}

        //[ModuleAction("editablemenu_getmenu", Module.PERM_EDITABLEMENU)]
        //public JsonResult EditableMenuLinks(string ModuleName = null)
        //{
        //    var success = false;
        //    var result = "";
        //    object data = null;

        //    try
        //    {
        //        if (string.IsNullOrEmpty(ModuleName)) throw new Exception("Не указано имя модуля.");

        //        var module = ModulesManager.getModuleByName(ModuleName) ?? ModulesManager.getModuleByNameBase(ModuleName);

        //        if (module == null) throw new Exception(string.Format("Не получилось найти модуль с именем '{0}' среди загруженных модулей.", ModuleName));

        //        var itemsAll = module.GetItemTypes().
        //                        SelectMany(x => module.GetItems(x.IdItemType)).
        //                        OrderBy(x => x.Caption).
        //                        ToDictionary(x => (x as Items.ItemBase).Url.ToString(), x => x.Caption);


        //        data = itemsAll;

        //        success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        result = ex.Message;
        //    }

        //    return ReturnJson(success, result, data);
        //}

        //[ModuleAction("editablemenu_edit_save", Module.PERM_EDITABLEMENU)]
        //public JsonResult EditableMenuSave(menus menu)
        //{
        //    var success = false;
        //    var result = "";

        //    try
        //    {
        //        if (menu == null) throw new Exception("Не удалось получить данные из формы.");

        //        var dbmenu = menu.id > 0 ? DB.EditableMenu.Where(x => x.id == menu.id).FirstOrDefault() : menu;
        //        if (dbmenu == null) throw new Exception(string.Format("Не удалось найти меню с id={0}", menu.id));

        //        if (string.IsNullOrEmpty(menu.name)) throw new Exception("Название меню не может быть пустым.");

        //        if (dbmenu.id == 0) DB.EditableMenu.Add(menu);
        //        else
        //        {
        //            dbmenu.name = menu.name;
        //            dbmenu.code = menu.code;
        //        }

        //        DB.SaveChanges();

        //        success = true;
        //        result = "Изменения в свойствах меню успешно сохранены.";
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        result = ex.Message;
        //    }

        //    return ReturnJson(success, result);
        //}

        //[ModuleAction("editablemenu_delete", Module.PERM_EDITABLEMENU)]
        //public ActionResult EditableMenuDelete(int IdMenu = 0)
        //{
        //    var success = false;
        //    var result = "";

        //    try
        //    {
        //        if (IdMenu == 0) throw new Exception("Следует указать номер существующего меню.");

        //        var menu = DB.EditableMenu.Where(x => x.id == IdMenu).FirstOrDefault();
        //        if (menu == null) throw new Exception(string.Format("Меню с id={0} не найдено.", IdMenu));

        //        DB.EditableMenu.Delete(menu);
        //        DB.SaveChanges();

        //        success = true;
        //        //result = "Меню было успешно удалено.";
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        result = ex.Message;
        //    }

        //    return ReturnJson(success, result);
        //}

        //[MenuAction("Sitemap", "sitemap", Module.PERM_SITEMAP)]
        //public ActionResult Sitemap()
        //{
        //    var model = (from p in ModulesManager.getModules() orderby p.getCaption() select p).ToList();
        //    return this.display("admin_adminmain_sitemap.tpl", model);
        //}

        //[ModuleAction("sitemap_save", Module.PERM_SITEMAP)]
        //public JsonResult SitemapGenerate(string[] modules_list)
        //{
        //    var success = false;
        //    var result = "";

        //    try
        //    {
        //        var linksAll = modules_list.SelectMany(moduleNameBase =>
        //        {
        //            var module = ModulesManager.getModuleByName(moduleNameBase) ?? ModulesManager.getModuleByNameBase(moduleNameBase);
        //            return module?.GetItemTypes().
        //                           Where(x => x != null).
        //                           Select(x => module.GetItems(x.IdItemType)).
        //                           Where(x => x != null).
        //                           SelectMany(x => x).
        //                           Where(x => x is Items.ItemBase).
        //                           OrderBy(x => x.Caption);
        //        }).Select(x => x as Items.ItemBase);

        //        var code = this.displayToVar("admin_adminmain_sitemap_templ.cshtml", linksAll);

        //        var path = System.IO.Path.Combine(this.Request.PhysicalApplicationPath, "sitemap.xml");
        //        System.IO.File.WriteAllText(path, code);

        //        success = true;
        //        result = "Файл sitemap.xml был успешно обновлен.";
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        result = ex.Message;
        //    }

        //    return ReturnJson(success, result);
        //}

        //[MenuAction("RSS", "rss", Module.PERM_RSS)]
        //public ActionResult RSS()
        //{
        //    var model = (from p in ModulesManager.getModules() orderby p.getCaption() select p).ToList();
        //    return this.display("admin_adminmain_rss.tpl", model);
        //}

        //[ModuleAction("rss_save", Module.PERM_RSS)]
        //public JsonResult RSSGenerate(string[] modules_list)
        //{
        //    var success = false;
        //    var result = "";

        //    try
        //    {
        //        var linksAll = modules_list.SelectMany(moduleNameBase =>
        //        {
        //            var module = ModulesManager.getModuleByName(moduleNameBase) ?? ModulesManager.getModuleByNameBase(moduleNameBase);
        //            return module?.GetItemTypes().
        //                           Where(x => x != null).
        //                           Select(x => module.GetItems(x.IdItemType)).
        //                           Where(x => x != null).
        //                           SelectMany(x => x).
        //                           Where(x => x is Items.ItemBase).
        //                           OrderBy(x => x.Caption);
        //        });

        //        var code = this.displayToVar("admin_adminmain_sitemap_templ.cshtml", linksAll);

        //        var path = System.IO.Path.Combine(this.Request.PhysicalApplicationPath, "sitemap.xml");
        //        System.IO.File.WriteAllText(path, code);

        //        success = true;
        //        result = "Файл sitemap.xml был успешно обновлен.";
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        result = ex.Message;
        //    }

        //    return ReturnJson(success, result);
        //}

        //[MenuAction("Адреса (КЛАДР)", "addresses", Module.PERM_ADDRESS)]
        //public ActionResult AddressSystem()
        //{
        //    if (Request.Form["changeSettings"] != null)
        //    {
        //        if (Request.Form["makeAddressMode"] != null)
        //        switch (Request.Form["makeAddressMode"].ToString())
        //        {
        //            case "1":
        //                AppCore.Config["makeAddressMode"] = 1;
        //                break;

        //            case "0":
        //            default:
        //                AppCore.Config["makeAddressMode"] = 0;

        //                break;
        //        }

        //        if (Request.Form.HasKey("kladrInCloudToken")) AppCore.Config["kladrInCloudToken"] = Request.Form["kladrInCloudToken"];
        //        if (Request.Form.HasKey("kladrInCloudKey")) AppCore.Config["kladrInCloudKey"] = Request.Form["kladrInCloudKey"];
        //        if (Request.Form.HasKey("mapsYandexKey")) AppCore.Config["mapsYandexKey"] = Request.Form["mapsYandexKey"];
        //        if (Request.Form.HasKey("dadataSecretKey")) AppCore.Config["dadataSecretKey"] = Request.Form["dadataSecretKey"];
        //        if (Request.Form.HasKey("dadataApiKey")) AppCore.Config["dadataApiKey"] = Request.Form["dadataApiKey"];

        //        AppCore.ConfigurationSave();
        //    }

        //    return this.display("admin_adminmain_addresses.tpl");
        //}

        //[MenuAction("Маршрутизация (ЧПУ)", "routing", Module.PERM_ROUTING)]
        //public virtual ActionResult Routing()
        //{
        //    var model = new Model.Routing() { Modules = ModulesManager.getModules().OrderBy(x => x.Caption).ToList() };

        //    var modulesIdList = model.Modules.Select(x => x.ID).ToArray();
        //    var query = DB.Routes
        //                    .Where(x => modulesIdList.Contains(x.IdModule))
        //                    .GroupBy(x => new { x.IdModule, x.IdRoutingType })
        //                    .Select(x => new { x.Key.IdModule, x.Key.IdRoutingType, Count = x.Count() })
        //                    .GroupBy(x => x.IdRoutingType)
        //                    .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.IdModule, y => y.Count))
        //                    ;

        //    var query2 = (new RoutingType.eTypes[] { RoutingType.eTypes.Main, RoutingType.eTypes.Additional, RoutingType.eTypes.Old })
        //                    .ToDictionary(x => x,
        //                                  x => model.Modules.ToDictionary(y => y.ID,
        //                                                                       y => query.ContainsKey(x) && query[x].ContainsKey(y.ID) ? query[x][y.ID] : 0));

        //    model.RoutesMain = query2[RoutingType.eTypes.Main];
        //    model.RoutesAdditional = query2[RoutingType.eTypes.Additional];
        //    model.RoutesOld = query2[RoutingType.eTypes.Old];

        //    return this.display("routing.cshtml", model);
        //}

        //[ModuleAction("routingModule", Module.PERM_ROUTING)]
        //public virtual ActionResult RoutingModule(int IdModule)
        //{
        //    if (IdModule <= 0) throw new Exception("Не указан идентификатор модуля.");
        //    var module = ModulesManager.getModuleByID(IdModule);

        //    if (module == null) throw new Exception($"Не получилось найти модуль с указанным идентификатором {IdModule}.");

        //    var model = new Model.RoutingModule() { Module = module };

        //    model.RoutingTypes = DB.RouteTypes.OrderBy(x => x.NameTranslationType).Select(x => new SelectListItem() { Value = x.IdTranslationType.ToString(), Text = x.NameTranslationType }).ToList();

        //    var moduleActionAttributeType = typeof(ModuleActionAttribute);

        //    var modulesActions = ModulesManager.getModules().Select(x => new
        //    {
        //        Module = x,
        //        UserDescriptor = x.ControllerUser() != null ? new ReflectedControllerDescriptor(x.ControllerUser()) : null,
        //        AdminDescriptor = x.ControllerAdmin() != null ? new ReflectedControllerDescriptor(x.ControllerAdmin()) : null,
        //    }).Select(x => new
        //    {
        //        x.Module,
        //        UserActions = x.UserDescriptor != null ? x.UserDescriptor.GetCanonicalActions().Where(y => y.IsDefined(moduleActionAttributeType, true)).ToDictionary(y => y.ActionName, y => "Общее: " + y.GetDisplayName()) : new Dictionary<string, string>(),
        //        AdminActions = x.AdminDescriptor != null ? x.AdminDescriptor.GetCanonicalActions().Where(y => y.IsDefined(moduleActionAttributeType, true)).ToDictionary(y => y.ActionName, y => "Администрирование: " + y.GetDisplayName()) : new Dictionary<string, string>(),
        //    }).ToDictionary(x => x.Module, x => x.UserActions.Merge(x.AdminActions).OrderBy(y => y.Value).ToDictionary(y => y.Key, y => y.Value));

        //    model.ModulesActions = modulesActions.
        //        Select(x => new { Group = new SelectListGroup() { Name = x.Key.Caption }, Items = x.Value, Module=x.Key }).
        //        SelectMany(x => x.Items.Select(y => new SelectListItem() { Text = y.Value, Value = $"{x.Module.ID}_{y.Key}", Group = x.Group })).ToList();

        //    model.Routes = DB.Routes
        //            .Where(x => x.IdModule == module.ID)
        //            .OrderBy(x => x.IdRoutingType)
        //            .ToList()
        //            .Select(x => new { Route = x, Item = Items.ItemTypeFactory.Instance.GetItemOfType(x.IdItemType, x.IdItem) })
        //            .Select(x => new Model.RouteInfo() { Route = x.Route, ItemName = x.Route.IdItem > 0 ? (x.Item != null ? x.Item.ToString() : "Не найден") : "Без объекта" })
        //            .ToList();

        //    return this.display("routing_module.cshtml", model);
        //}

        //[MenuAction("Рассылки и уведомления", "messaging", Module.PERM_MANAGE_MESSAGING)]
        //public virtual ActionResult Messaging()
        //{
        //    //Web.AppCore.Get<Core.Messaging.Email.IService>().sendMailFromSite("test", "test@test.ru", "test", "test!!!!!!!!");

        //    return this.display("Messaging.cshtml");
        //}

        //public virtual ActionResult MessagingTestEmail()
        //{
        //    var answer = JsonAnswer();

        //    try
        //    {

        //    }
        //    catch (Exception ex) { answer.FromException(ex); }

        //    return ReturnJson(answer);
        //}

        //[MenuAction("Состояние системы", "monitor")]
        //public virtual ActionResult Monitor()
        //{
        //    return this.display("Monitor.cshtml");
        //}

        //public virtual ActionResult MonitorJournal(Guid? serviceGuid = null)
        //{
        //    try
        //    {
        //        if (!serviceGuid.HasValue) throw new Exception("Не указан идентификатор сервиса.");

        //        var serviceJournal = ServiceMonitor.Monitor.GetServiceJournal(serviceGuid.Value);
        //        return this.display("MonitorJournal.cshtml", serviceJournal.ToList());
        //    }
        //    catch (Exception ex)
        //    {
        //        var answer = JsonAnswer();
        //        answer.FromException(ex);
        //        return ReturnJson(answer);
        //    }
        //}

        //[MenuAction("Журналы системы", "journals")]
        //public virtual ActionResult Journals()
        //{
        //    using (var db = new UnitOfWork<DB.JournalName, DB.Journal>())
        //    {
        //        var query = from p in db.Repo2
        //                    group p by p.IdJournal into gr
        //                    select new { IdJournal = gr.Key, Count = gr.Count(), IdJournalDataLast = gr.Max(x => x.IdJournalData) };

        //        //var datetimeDefault = 

        //        var data = (from p in db.Repo1
        //                    join d in query on p.IdJournal equals d.IdJournal
        //                    join d2 in db.Repo2 on d.IdJournalDataLast equals d2.IdJournalData into d2_j
        //                    from d2 in d2_j.DefaultIfEmpty()
        //                    orderby p.Name
        //                    select new Design.Model.JournalsList()
        //                    {
        //                        JournalName = p,
        //                        EventsCount = d == null ? 0 : d.Count,
        //                        EventLastDate = d2 == null ? null : (DateTime?)d2.DateEvent,
        //                        EventLastType = d2 == null ? null : (Journaling.EventType?)d2.EventType
        //                    }).ToList();

        //        return this.display("Journals.cshtml", data);
        //    }
        //}

        //public virtual ActionResult JournalDetails(int? IdJournal = null)
        //{
        //    try
        //    {
        //        if (!IdJournal.HasValue) throw new Exception("Не указан идентификатор журнала.");

        //        using (var scope = TransactionsHelper.ReadUncommited())
        //        {
        //            var result = Journaling.Manager.GetJournal(IdJournal.Value);
        //            if (!result.IsSuccess) throw new Exception(result.Message);

        //            using (var db = new UnitOfWork<DB.Journal>())
        //            {
        //                int skip = 0;
        //                int limit = 100;

        //                var data = db.Repo1.Where(x => x.IdJournal == result.Result.IdJournal).OrderByDescending(x => x.DateEvent).Skip(skip).Take(limit).ToList();
        //                return View("JournalDetails.cshtml", new Design.Model.JournalDetails()
        //                {
        //                    JournalName = result.Result,
        //                    JournalData = data
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var answer = JsonAnswer();
        //        answer.FromException(ex);
        //        return ReturnJson(answer);
        //    }
        //}

        //public virtual ActionResult JournalClear(int? IdJournal = null)
        //{
        //    var answer = JsonAnswer();

        //    try
        //    {
        //        if (!IdJournal.HasValue) throw new Exception("Не указан идентификатор журнала.");

        //        using (var db = new UnitOfWork<DB.Journal>())
        //        using (var scope = db.CreateScope())
        //        {
        //            var result = Journaling.Manager.GetJournal(IdJournal.Value);
        //            if (!result.IsSuccess) throw new Exception(result.Message);

        //            db.DataContext.ExecuteQuery("DELETE FROM Journal WHERE IdJournal=@IdJournal", new { IdJournal = result.Result.IdJournal });

        //            scope.Commit();
        //        }
        //        answer.FromSuccess(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.RegisterEventWithCode(System.Net.HttpStatusCode.InternalServerError, "Ошибка во время удаления журнала", $"Журнал №{IdJournal}", ex);
        //        answer.FromFail("Ошибка во время удаления журнала.");
        //    }
        //    return ReturnJson(answer);
        //}

    }

}