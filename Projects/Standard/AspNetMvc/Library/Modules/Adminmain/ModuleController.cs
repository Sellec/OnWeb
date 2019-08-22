using OnUtils.Application.Configuration;
using OnUtils.Application.DB;
using OnUtils.Application.Items;
using OnUtils.Application.Journaling;
using JournalingDB = OnUtils.Application.Journaling.DB;
using OnUtils.Application.Modules.CoreModule;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Modules.Adminmain
{
    using AdminForModules.Menu;
    using Core.Modules;
    using Binding.Routing;
    using Journaling;
    using Routing.DB;
    using WebCoreModule;

    /// <summary>
    /// Представляет контроллер для панели управления.
    /// </summary>
    public class ModuleController : ModuleControllerAdmin<Module>
    {
        [MenuAction("Настройки", "info", Module.PERM_CONFIGMAIN)]
        public virtual ActionResult MainSettings()
        {
            var handler = AppCore.Get<ModuleControllerTypesManager>();
            var model = new Model.AdminMainModelInfoPage(AppCore.AppConfig, AppCore.WebConfig)
            {
                ModulesList = (from p in AppCore.GetModulesManager().GetModules()
                               where handler.GetModuleControllerTypes(p.QueryType) != null
                               orderby p.Caption
                               select new SelectListItem()
                               {
                                   Value = p.ID.ToString(),
                                   Text = p.Caption,
                                   Selected = AppCore.WebConfig.IdModuleDefault == p.ID
                               }).ToList()
            };

            using (var db = Module.CreateUnitOfWork())
            {
                model.Roles = (from p in db.Role orderby p.NameRole ascending select p).ToList();
                model.Roles.Insert(0, new Role() { IdRole = 0, NameRole = "Не выбрано" });
            }

            return View("CoreSettings.tpl", model);
        }

        [ModuleAction("info_save", Module.PERM_CONFIGMAIN)]
        public virtual JsonResult MainSettingsSave(Model.AdminMainModelInfoPage model)
        {
            var result = JsonAnswer();
            CoreConfiguration<WebApplication> cfgAppOld = null;
            WebCoreConfiguration cfgWebOld = null;

            try
            {
                if (ModelState.IsValid)
                {
                    cfgAppOld = AppCore.AppCoreModule.GetConfigurationManipulator().GetEditable<CoreConfiguration<WebApplication>>();
                    cfgWebOld = AppCore.GetModulesManager().GetModule<WebCoreModule>().GetConfigurationManipulator().GetEditable<WebCoreConfiguration>();

                    var cfgApp = AppCore.GetModulesManager().GetModule<CoreModule<WebApplication>>().GetConfigurationManipulator().GetEditable<CoreConfiguration<WebApplication>>();
                    cfgApp.RoleGuest = model.AppCoreConfiguration.RoleGuest;
                    cfgApp.RoleUser = model.AppCoreConfiguration.RoleUser;

                    var applyResult = AppCore.GetModulesManager().GetModule<CoreModule<WebApplication>>().GetConfigurationManipulator().ApplyConfiguration(cfgApp);
                    switch (applyResult.Item1)
                    {
                        case ApplyConfigurationResult.PermissionDenied:
                            result.Message = "Недостаточно прав для сохранения конфигурации системы.";
                            result.Success = false;
                            break;

                        case ApplyConfigurationResult.Failed:
                            var journalData = AppCore.Get<JournalingManager>().GetJournalData(applyResult.Item2.Value);
                            result.Message = $"Возникла ошибка при сохранении настроек: {(journalData?.Message ?? "текст ошибки не найден")}.";
                            result.Success = false;
                            break;

                        case ApplyConfigurationResult.Success:
                            System.Web.Routing.RouteTable.Routes.Where(x => x is RouteWithDefaults).Select(x => x as RouteWithDefaults).ForEach(x => x.UpdateDefaults());

                            result.Message = "Сохранено успешно!";
                            result.Success = true;
                            break;
                    }

                    if (result.Success)
                    {
                        var cfg = AppCore.GetModulesManager().GetModule<WebCoreModule>().GetConfigurationManipulator().GetEditable<WebCoreConfiguration>();
                        cfg.IdModuleDefault = model.WebCoreConfiguration.IdModuleDefault;
                        cfg.DeveloperEmail = model.WebCoreConfiguration.DeveloperEmail;
                        cfg.SiteFullName = model.WebCoreConfiguration.SiteFullName;
                        cfg.ContactEmail = model.WebCoreConfiguration.ContactEmail;
                        cfg.ReturnEmail = model.WebCoreConfiguration.ReturnEmail;
                        cfg.CriticalMessagesEmail = model.WebCoreConfiguration.CriticalMessagesEmail;
                        cfg.register_mode = model.WebCoreConfiguration.register_mode;
                        cfg.site_reginfo = model.WebCoreConfiguration.site_reginfo;
                        cfg.site_loginfo = model.WebCoreConfiguration.site_loginfo;
                        cfg.help_info = model.WebCoreConfiguration.help_info;
                        cfg.site_descr = model.WebCoreConfiguration.site_descr;
                        cfg.site_keys = model.WebCoreConfiguration.site_keys;

                        cfg.userAuthorizeAllowed = model.WebCoreConfiguration.userAuthorizeAllowed;

                        applyResult = AppCore.GetModulesManager().GetModule<WebCoreModule>().GetConfigurationManipulator().ApplyConfiguration(cfg);
                        switch (applyResult.Item1)
                        {
                            case ApplyConfigurationResult.PermissionDenied:
                                result.Message = "Недостаточно прав для сохранения конфигурации системы.";
                                result.Success = false;
                                break;

                            case ApplyConfigurationResult.Failed:
                                var journalData = AppCore.Get<JournalingManager>().GetJournalData(applyResult.Item2.Value);
                                result.Message = $"Возникла ошибка при сохранении настроек: {(journalData?.Message ?? "текст ошибки не найден")}.";
                                result.Success = false;
                                break;

                            case ApplyConfigurationResult.Success:
                                System.Web.Routing.RouteTable.Routes.Where(x => x is RouteWithDefaults).Select(x => x as RouteWithDefaults).ForEach(x => x.UpdateDefaults());

                                result.Message = "Сохранено успешно!";
                                result.Success = true;
                                break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            try
            {
                if (!result.Success)
                {
                    if (cfgAppOld != null) AppCore.GetModulesManager().GetModule<CoreModule<WebApplication>>().GetConfigurationManipulator().ApplyConfiguration(cfgAppOld);
                    if (cfgWebOld != null) AppCore.GetModulesManager().GetModule<WebCoreModule>().GetConfigurationManipulator().ApplyConfiguration(cfgWebOld);
                }
            }
            catch { }

            return this.ReturnJson(result);
        }

        [MenuAction("Модули", "modules", Module.PERM_MODULES)]
        public ActionResult ModulesList()
        {
            var model = new Model.Modules()
            {
                //Unregistered = (from p in AppCore.GetModulesManager().getModulesCandidates()
                //                orderby p.Key.Info.ModuleCaption
                //                select new Model.ModuleUnregistered(p.Key) { Info = p.Value }).ToList(),
                Registered = (from p in AppCore.GetModulesManager().GetModules().OfType<IModuleCore>()
                              orderby p.Caption
                              select new Model.Module(p)).ToList()
            };

            return View("Modules.cshtml", model);
        }

        [MenuAction("Маршрутизация (ЧПУ)", "routing", Module.PERM_ROUTING)]
        public virtual ActionResult Routing()
        {
            var model = new Model.Routing() { Modules = AppCore.GetModulesManager().GetModules().OfType<IModuleCore>().OrderBy(x => x.Caption).ToList() };

            using (var db = new UnitOfWork<Routing>())// Module.CreateUnitOfWork())
            {
                var modulesIdList = model.Modules.Select(x => x.IdModule).ToArray();
                var query = db.Repo1
                                .Where(x => modulesIdList.Contains(x.IdModule))
                                .GroupBy(x => new { x.IdModule, x.IdRoutingType })
                                .Select(x => new { x.Key.IdModule, x.Key.IdRoutingType, Count = x.Count() })
                                .GroupBy(x => x.IdRoutingType)
                                .ToDictionary(x => x.Key, x => x.ToDictionary(y => y.IdModule, y => y.Count))
                                ;

                var query2 = (new RoutingType.eTypes[] { RoutingType.eTypes.Main, RoutingType.eTypes.Additional, RoutingType.eTypes.Old })
                                .ToDictionary(x => x,
                                              x => model.Modules.ToDictionary(y => y.IdModule,
                                                                                   y => query.ContainsKey(x) && query[x].ContainsKey(y.IdModule) ? query[x][y.IdModule] : 0));

                model.RoutesMain = query2[RoutingType.eTypes.Main];
                model.RoutesAdditional = query2[RoutingType.eTypes.Additional];
                model.RoutesOld = query2[RoutingType.eTypes.Old];
            }

            return View("routing.cshtml", model);
        }

        [ModuleAction("routingModule", Module.PERM_ROUTING)]
        public virtual ActionResult RoutingModule(int IdModule)
        {
            if (IdModule <= 0) throw new Exception("Не указан идентификатор модуля.");
            var module = AppCore.GetModulesManager().GetModule(IdModule);

            if (module == null) throw new Exception($"Не получилось найти модуль с указанным идентификатором {IdModule}.");

            var model = new Model.RoutingModule() { Module = (IModuleCore)module };

            using (var db = new UnitOfWork<Routing, RoutingType>())// Module.CreateUnitOfWork())
            {
                model.RoutingTypes = db.Repo2.OrderBy(x => x.NameTranslationType).Select(x => new SelectListItem() { Value = x.IdTranslationType.ToString(), Text = x.NameTranslationType }).ToList();

                var moduleActionAttributeType = typeof(ModuleActionAttribute);
                var moduleActionGetDisplayName = new Func<ActionDescriptor, string>(action =>
                {
                    var attr = action.GetCustomAttributes(moduleActionAttributeType, true).OfType<ModuleActionAttribute>().FirstOrDefault();
                    if (attr != null)
                    {
                        if (!string.IsNullOrEmpty(attr.Caption)) return attr.Caption;
                    }

                    return action.ActionName;
                });

                var modulesActions = AppCore.GetModulesManager().GetModules().OfType<IModuleCore>().Select(x => new
                {
                    Module = x,
                    UserDescriptor = x.ControllerUser() != null ? new ReflectedControllerDescriptor(x.ControllerUser()) : null,
                    AdminDescriptor = x.ControllerAdmin() != null ? new ReflectedControllerDescriptor(x.ControllerAdmin()) : null,
                }).Select(x => new
                {
                    x.Module,
                    UserActions = x.UserDescriptor != null ? x.UserDescriptor.GetCanonicalActions().Where(y => y.IsDefined(moduleActionAttributeType, true)).ToDictionary(y => y.ActionName, y => "Общее: " + moduleActionGetDisplayName(y)) : new Dictionary<string, string>(),
                    AdminActions = x.AdminDescriptor != null ? x.AdminDescriptor.GetCanonicalActions().Where(y => y.IsDefined(moduleActionAttributeType, true)).ToDictionary(y => y.ActionName, y => "Администрирование: " + moduleActionGetDisplayName(y)) : new Dictionary<string, string>(),
                }).ToDictionary(x => x.Module, x => x.UserActions.Merge(x.AdminActions).OrderBy(y => y.Value).ToDictionary(y => y.Key, y => y.Value));

                model.ModulesActions = modulesActions.
                    Select(x => new { Group = new SelectListGroup() { Name = x.Key.Caption }, Items = x.Value, Module = x.Key }).
                    SelectMany(x => x.Items.Select(y => new SelectListItem() { Text = y.Value, Value = $"{x.Module.IdModule}_{y.Key}", Group = x.Group })).ToList();

                model.Routes = db.Repo1
                        .Where(x => x.IdModule == module.ID)
                        .OrderBy(x => x.IdRoutingType)
                        .ToList()
                        .Select(x => new { Route = x, Item = ItemTypeFactory.GetItemOfType(x.IdItemType, x.IdItem) })
                        .Select(x => new Model.RouteInfo() { Route = x.Route, ItemName = x.Route.IdItem > 0 ? (x.Item != null ? x.Item.ToString() : "Не найден") : "Без объекта" })
                        .ToList();
            }

            return View("routing_module.cshtml", model);
        }

        [MenuAction("Рассылки и уведомления", "messaging", Module.PERM_MANAGE_MESSAGING)]
        public virtual ActionResult Messaging()
        {
            //Web.AppCore.Get<Core.Messaging.Email.IService>().sendMailFromSite("test", "test@test.ru", "test", "test!!!!!!!!");

            return View("Messaging.cshtml");
        }

        public virtual ActionResult MessagingTestEmail()
        {
            var answer = JsonAnswer();

            try
            {

            }
            catch (Exception ex) { answer.FromException(ex); }

            return ReturnJson(answer);
        }

        [MenuAction("Состояние системы", "monitor")]
        public virtual ActionResult Monitor()
        {
            return View("Monitor.cshtml");
        }

        public virtual ActionResult MonitorJournal(Guid? serviceGuid = null)
        {
            try
            {
                if (!serviceGuid.HasValue) throw new Exception("Не указан идентификатор сервиса.");

                var serviceJournal = AppCore.Get<ServiceMonitor.Monitor>().GetServiceJournal(serviceGuid.Value);
                return View("MonitorJournal.cshtml", serviceJournal.ToList());
            }
            catch (Exception ex)
            {
                var answer = JsonAnswer();
                answer.FromException(ex);
                return ReturnJson(answer);
            }
        }

        [MenuAction("Журналы системы", "journals")]
        public virtual ActionResult Journals()
        {
            using (var db = new JournalingDB.DataContext())
            {
                var dbAccessor = AppCore.Get<JournalingDB.JournalingManagerDatabaseAccessor<WebApplication>>();

                var queryDataBase = dbAccessor.CreateQueryJournalData(db);
                var queryDataGrouped = from row in queryDataBase
                                       group row.JournalData by row.JournalName.IdJournal into gr
                                       select new { Count = gr.Count(), IdJournalDataLast = gr.Max(x => x.IdJournalData) };

                var query = from row in queryDataBase
                            join sq2 in queryDataGrouped on row.JournalData.IdJournalData equals sq2.IdJournalDataLast
                            select new Model.JournalQueries.QueryJournalData
                            {
                                JournalData = row.JournalData,
                                JournalName = row.JournalName,
                                User = row.User,
                                Count = sq2.Count
                            };

                var data = dbAccessor.
                    FetchQueryJournalData<Model.JournalQueries.QueryJournalData, Model.JournalQueries.JournalData>(query, (row, instance) => instance.Count = row.Count).
                    Select(x => new Design.Model.JournalsList()
                    {
                        JournalName = x.JournalInfo,
                        EventsCount = x.Count,
                        EventLastDate = x.DateEvent,
                        EventLastType = x.EventType
                    }).ToList();

                return View("Journals.cshtml", data);
            }
        }

        public virtual ActionResult JournalDetails(int? IdJournal = null)
        {
            try
            {
                if (!IdJournal.HasValue) throw new Exception("Не указан идентификатор журнала.");

                using (var scope = TransactionsHelper.ReadUncommited())
                {
                    var result = AppCore.Get<JournalingManager>().GetJournal(IdJournal.Value);
                    if (!result.IsSuccess) throw new Exception(result.Message);

                    var dbAccessor = AppCore.Get<JournalingDB.JournalingManagerDatabaseAccessor<WebApplication>>();

                    using (var db = new JournalingDB.DataContext())
                    {
                        int skip = 0;
                        int limit = 100;

                        var query = dbAccessor.CreateQueryJournalData(db).Where(x => x.JournalData.IdJournal == result.Result.IdJournal).OrderByDescending(x => x.JournalData.DateEvent).Skip(skip).Take(limit);
                        var data = dbAccessor.FetchQueryJournalData(query);
                        return View("JournalDetails.cshtml", new Design.Model.JournalDetails()
                        {
                            JournalName = result.Result,
                            JournalData = data
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                var answer = JsonAnswer();
                answer.FromException(ex);
                return ReturnJson(answer);
            }
        }

        public virtual ActionResult JournalClear(int? IdJournal = null)
        {
            var answer = JsonAnswer();

            try
            {
                if (!IdJournal.HasValue) throw new Exception("Не указан идентификатор журнала.");

                var result = AppCore.Get<JournalingManager>().GetJournal(IdJournal.Value);
                if (!result.IsSuccess) throw new Exception(result.Message);

                using (var db = new JournalingDB.DataContext())
                using (var scope = db.CreateScope())
                {
                    db.DataContext.ExecuteQuery("DELETE FROM Journal WHERE IdJournal=@IdJournal", new { IdJournal = result.Result.IdJournal });
                    scope.Commit();
                }
                answer.FromSuccess(null);
            }
            catch (Exception ex)
            {
                this.RegisterEventWithCode(System.Net.HttpStatusCode.InternalServerError, "Ошибка во время удаления журнала", $"Журнал №{IdJournal}", ex);
                answer.FromFail("Ошибка во время удаления журнала.");
            }
            return ReturnJson(answer);
        }

    }
}
