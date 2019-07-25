using OnUtils.Application.DB;
using OnUtils.Application.Items;
using OnUtils.Application.Modules;
using OnUtils.Application.Modules.ItemsCustomize.DB;
using OnUtils.Application.Modules.ItemsCustomize.Field;
using OnUtils.Application.Modules.ItemsCustomize.Scheme;
using OnUtils.Application.Modules.ItemsCustomize;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.ItemsCustomize
{
    using Core.Types;
    using Core.Modules;
    using CoreBind.Modules;
    using Model;

    /// <summary>
    /// Админский класс расширения пользовательских полей. 
    /// </summary>
    class ModuleController : ModuleControllerAdmin<ModuleItemsCustomize>, IUnitOfWorkAccessor<Context>
    {
        [ModuleAction("fields")]
        public ActionResult FieldsList(int? idModule = null)
        {
            if (!idModule.HasValue) throw new Exception("Не указан идентификатор модуля.");

            var module = AppCore.GetModulesManager().GetModule(idModule.Value) as IModuleCore;
            if (module == null) throw new Exception("Модуль не найден.");

            AppCore.Get<ModuleItemsCustomize<WebApplicationBase>>().UpdateCache();

            var schemes = Module.GetSchemeList(idModule.Value);
            var schemeItems = Module.GetSchemeItemsList(idModule.Value);

            var id = idModule.Value;

            using (var db = this.CreateUnitOfWork())
            {
                var fields = (from p in db.CustomFieldsFields
                              where p.Block == 0 && p.IdModule == id
                              orderby p.name
                              select p).ToDictionary(x => x.IdField, x => x as IField);

                return View("SchemesEdit.cshtml", new Fields
                {
                    IdModule = idModule.Value,
                    FieldsList = fields,
                    SchemeItems = schemeItems,
                    Schemes = schemes,
                    AllowSchemesManage = Module.CheckPermission(ModuleItemsCustomize<WebApplicationBase>.PERM_EXTFIELDS_ALLOWMANAGE) == CheckPermissionResult.Allowed
                });
            }
        }

        [ModuleAction("fieldsItem")]
        public ActionResult ContainerItem(int? idModule = null, int idSchemeItem = 0, int idSchemeItemType = 0)
        {
            if (!idModule.HasValue) throw new Exception("Не указан идентификатор модуля.");

            var module = AppCore.GetModulesManager().GetModule(idModule.Value) as IModuleCore;
            if (module == null) throw new Exception("Модуль не найден.");

            var schemeItem = new SchemeItem(idSchemeItem, idSchemeItemType);

            using (var db = this.CreateUnitOfWork())
            {
                var fields = db.CustomFieldsFields.Where(x => x.IdModule == idModule.Value && x.Block == 0).ToDictionary(x => x.IdField, x => x);

                var schemes = db.CustomFieldsSchemes.
                    Where(x => x.IdModule == idModule.Value && x.IdScheme > 0).
                    OrderBy(x => x.NameScheme).
                    ToDictionary(x => (uint)x.IdScheme, x => new SchemeContainerItem.Scheme() { Name = x.NameScheme, Fields = new Dictionary<int, IField>() });

                schemes[0] = new SchemeContainerItem.Scheme() { Name = "По-умолчанию", Fields = new Dictionary<int, IField>() };

                var sql = (from datas in db.CustomFieldsSchemeDatas
                           where datas.IdModule == idModule.Value && datas.IdItemType == schemeItem.IdItemType && datas.IdSchemeItem == schemeItem.IdItem
                           group new { datas.IdField, datas.Order } by datas.IdScheme into gr
                           select new { IdScheme = gr.Key, Fields = gr.OrderBy(x => x.Order).Select(x => x.IdField).ToList() });

                var fieldsWithSchemes = sql.
                                         ToList().
                                         ToDictionary(x => (uint)x.IdScheme,
                                                      x => x.Fields.GroupBy(IdField => IdField).
                                                                Select(group => fields.GetValueOrDefault(group.Key)).
                                                                Where(field => field != null).
                                                                ToDictionary(field => field.IdField, field => field as IField)
                                         );

                var model = new SchemeContainerItem()
                {
                    IdModule = idModule.Value,
                    SchemeItem = schemeItem
                };

                if (fieldsWithSchemes.ContainsKey(0)) schemes[0].Fields = fieldsWithSchemes[0];
                foreach (var p in schemes)
                {
                    model.Schemes[p.Key] = p.Value;

                    if (fieldsWithSchemes.ContainsKey(p.Key)) p.Value.Fields = fieldsWithSchemes[p.Key];
                    int i = 0;
                    p.Value.Fields = p.Value.Fields.Where(x => schemes[0].Fields.ContainsKey(x.Key)).ToDictionary(x => i++, x => x.Value);
                }

                return View("Item.cshtml", model);
            }
        }

        [ModuleAction("fieldsItemSave")]
        public JsonResult ContainerItemSave(int? idModule = null, int idSchemeItem = 0, int idSchemeItemType = 0, Dictionary<string, int[]> model = null)
        {
            var result = JsonAnswer();

            try
            {
                if (!idModule.HasValue) throw new Exception("Не указан идентификатор модуля.");

                var module = AppCore.GetModulesManager().GetModule(idModule.Value) as IModuleCore;
                if (module == null) throw new Exception("Модуль не найден.");

                var schemeItem = new SchemeItem(idSchemeItem, idSchemeItemType);

                using (var db = Module.CreateUnitOfWork())
                using (var scope = db.CreateScope())
                {
                    db.CustomFieldsSchemeDatas.Where(x => x.IdModule == idModule.Value && x.IdSchemeItem == schemeItem.IdItem && x.IdItemType == schemeItem.IdItemType).Delete();

                    int k = 0;
                    var modelPrepared = model != null ? model.Where(x => int.TryParse(x.Key, out k)).ToDictionary(x => uint.Parse(x.Key), x => new List<int>(x.Value)) : null;
                    if (modelPrepared != null)
                    {
                        if (!modelPrepared.ContainsKey(0)) modelPrepared[0] = new List<int>();
                        foreach (var key in modelPrepared.Keys.ToList())
                        {
                            if (key == 0) continue;
                            var list = modelPrepared[key].Where(x => modelPrepared[0].Contains(x)).ToList();
                            modelPrepared[key] = list;
                        }

                        foreach (var pair in modelPrepared)
                        {
                            int i = 0;
                            foreach (var field in pair.Value)
                                db.CustomFieldsSchemeDatas.Add(new CustomFieldsSchemeData()
                                {
                                    IdModule = idModule.Value,
                                    IdScheme = (int)pair.Key,
                                    IdField = field,
                                    IdSchemeItem = schemeItem.IdItem,
                                    IdItemType = schemeItem.IdItemType,
                                    Order = i++
                                });
                        }

                        db.SaveChanges();
                    }

                    scope.Commit();

                    result.Message = "Сохранение расположения полей прошло успешно.";
                    result.Success = true;
                }
                AppCore.Get<ModuleItemsCustomize<WebApplicationBase>>().UpdateCache();
            }
            catch (Exception ex) { result.Message = ex.Message; }

            return ReturnJson(result);
        }

        [ModuleAction("fields_scheme_add")]
        public JsonResult SchemeAdd(int? idModule = null, string schemeName = null)
        {
            var result = JsonAnswer<uint>();

            try
            {
                if (!idModule.HasValue) throw new Exception("Не указан идентификатор модуля.");

                var module = AppCore.GetModulesManager().GetModule(idModule.Value) as IModuleCore;
                if (module == null) throw new Exception("Модуль не найден.");

                if (Request.Form.HasKey(nameof(schemeName))) schemeName = Request.Form[nameof(schemeName)];

                if (string.IsNullOrEmpty(schemeName)) result.Message = "Не указано название схемы!";
                else if (!schemeName.isOneStringTextOnly()) result.Message = "Некорректно указано название схемы!";
                else
                {
                    using (var db = this.CreateUnitOfWork())
                    {
                        var data = new CustomFieldsScheme()
                        {
                            IdModule = idModule.Value,
                            NameScheme = schemeName
                        };
                        db.CustomFieldsSchemes.Add(data);

                        if (db.SaveChanges() > 0)
                        {
                            result.Message = "Схема добавлена.";
                            result.Success = true;
                            result.Data = (uint)data.IdScheme;
                            AppCore.Get<ModuleItemsCustomize<WebApplicationBase>>().UpdateCache();
                        }
                        else result.Message = "По неизвестной причине схему не получилось добавить.";
                    }
                }
            }
            catch (Exception ex) { result.Message = ex.Message; }

            return ReturnJson(result);
        }

        [ModuleAction("fields_scheme_delete")]
        public JsonResult SchemeDelete(int IdScheme)
        {
            var result = JsonAnswer<int>();

            try
            {
                if (IdScheme == 0) result.Message = "Схема \"По-умолчанию\" не удаляется.";
                else
                {
                    using (var db = this.CreateUnitOfWork())
                    using (var scope = db.CreateScope())
                    {
                        var data = db.CustomFieldsSchemes.Where(x => x.IdScheme == IdScheme).FirstOrDefault();
                        if (data == null) throw new Exception(string.Format("Схема с номером '{0}' не найдена.", IdScheme));

                        db.CustomFieldsSchemeDatas.Where(x => x.IdScheme == IdScheme).Delete();
                        db.CustomFieldsSchemes.Where(x => x.IdScheme == IdScheme).Delete();

                        if (db.SaveChanges() > 0)
                        {
                            result.Message = "Схема удалена.";
                            result.Success = true;
                            scope.Commit();
                            AppCore.Get<ModuleItemsCustomize<WebApplicationBase>>().UpdateCache();
                        }
                        else result.Message = "По неизвестной причине схему не получилось удалить.";
                    }
                }
            }
            catch (Exception ex) { result.Message = ex.Message; }

            return ReturnJson(result);
        }

        [ModuleAction("fieldEdit")]
        public ActionResult FieldEdit(int? idModule = null, int idField = 0)
        {
            if (!idModule.HasValue) throw new Exception("Не указан идентификатор модуля.");

            var module = AppCore.GetModulesManager().GetModule(idModule.Value) as IModuleCore;
            if (module == null) throw new Exception("Модуль не найден.");

            CustomFieldsField data = null;
            if (idField > 0)
            {
                using (var db = this.CreateUnitOfWork())
                {
                    data = db.CustomFieldsFields.Where(x => x.IdField == idField).Include(x => x.data).FirstOrDefault();
                    if (data == null) throw new Exception("Такое поле не найдено в базе данных!");
                    if (data.IdModule != idModule.Value)
                    {
                        var module2 = AppCore.GetModulesManager().GetModule(data.IdModule);
                        if (module2 == null) throw new Exception("Это поле относится к другому модулю.");
                        else throw new Exception(string.Format("Это поле относится к модулю '{0}'.", module2.Caption));
                    }
                }
            }
            else
            {
                data = new CustomFieldsField() { IdFieldType = 0, IdModule = idModule.Value };
            }

            return View("FieldEdit.cshtml", new FieldEdit(data));
        }

        [ModuleAction("fieldSave")]
        public JsonResult FieldSave(FieldEdit model = null)
        {
            var result = JsonAnswer();

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    if (model.IdModule <= 0) throw new Exception("Не указан идентификатор модуля.");

                    var module = AppCore.GetModulesManager().GetModule(model.IdModule) as IModuleCore;
                    if (module == null) throw new Exception("Модуль не найден.");

                    CustomFieldsField data = null;
                    if (model.IdField > 0)
                    {
                        data = db.CustomFieldsFields.Where(x => x.IdField == model.IdField).Include(x => x.data).FirstOrDefault();
                        if (data == null) throw new Exception("Такое поле не найдено в базе данных!");
                        if (data.IdModule != model.IdModule)
                        {
                            var module2 = AppCore.GetModulesManager().GetModule(data.IdModule);
                            if (module2 == null) throw new Exception("Это поле относится к другому модулю.");
                            else throw new Exception(string.Format("Это поле относится к модулю '{0}'.", module2.Caption));
                        }
                    }
                    else
                    {
                        data = new CustomFieldsField() { IdFieldType = 0, IdModule = model.IdModule };
                        db.CustomFieldsFields.Add(data);
                    }

                    if (ModelState.ContainsKeyCorrect(nameof(model.name))) data.name = model.name;
                    if (ModelState.ContainsKeyCorrect(nameof(model.nameAlt))) data.nameAlt = model.nameAlt;
                    if (ModelState.ContainsKeyCorrect(nameof(model.alias))) data.alias = model.alias;
                    if (ModelState.ContainsKeyCorrect(nameof(model.formatCheck))) data.formatCheck = model.formatCheck;
                    if (ModelState.ContainsKeyCorrect(nameof(model.IdFieldType))) data.IdFieldType = model.IdFieldType;
                    if (ModelState.ContainsKeyCorrect(nameof(model.IsValueRequired))) data.IsValueRequired = model.IsValueRequired;
                    if (ModelState.ContainsKeyCorrect(nameof(model.IdValueType))) data.IdValueType = model.IdValueType;
                    if (ModelState.ContainsKeyCorrect(nameof(model.size))) data.size = model.size;
                    if (ModelState.ContainsKeyCorrect(nameof(model.nameEnding))) data.nameEnding = model.nameEnding;

                    using (var scope = db.CreateScope())
                    {
                        var entry = db.GetState(data);
                        if (entry == ItemState.Detached) throw new Exception("Невозможно найти поле для сохранения.");
                        else if (entry.In(ItemState.Modified, ItemState.Added))
                        {
                            data.DateChange = DateTime.Now.Timestamp();
                            data.Block = 0;

                            if (db.SaveChanges<CustomFieldsField>() == 0) throw new Exception("Сохранение поля провалилось!");
                        }

                        if (Request.Form.HasKey("values"))
                        {
                            var t = DateTime.Now.Timestamp();
                            var vals = Request.Form.TryGetValue<CustomFieldsValue[]>("values");

                            if (vals != null)
                            {
                                foreach (var d in vals)
                                {
                                    d.IdField = data.IdField;
                                    //d.Field = data;
                                    d.DateChange = t;
                                }

                                db.CustomFieldsValues.AddOrUpdate(vals);
                            }
                            else vals = new CustomFieldsValue[0];

                            var values = vals.ToList();

                            if (values.Count > 0)
                            {
                                if (db.SaveChanges<CustomFieldsValue>() == 0) throw new Exception("Не удалось сохранить варианты значений поля");
                            }

                            data.data = data.FieldType.CreateValuesCollection(data, values);

                            var keys = values.Select(x => x.IdFieldValue);

                            var removeExistsValue = new Dictionary<int, string>();
                            var removeExistsObjects = new Dictionary<int, List<int>>();

                            var sql = (from p in db.CustomFieldsValues
                                       join d in db.CustomFieldsDatas on p.IdFieldValue equals d.IdFieldValue
                                       where p.IdField == data.IdField && !keys.Contains(p.IdFieldValue)
                                       select new { p, d });

                            foreach (var res in sql)
                            {
                                removeExistsValue[res.p.IdFieldValue] = res.p.FieldValue;
                                if (!removeExistsObjects.ContainsKey(res.p.IdFieldValue)) removeExistsObjects[res.p.IdFieldValue] = new List<int>();
                                if (!removeExistsObjects[res.p.IdFieldValue].Contains(res.d.IdItem)) removeExistsObjects[res.p.IdFieldValue].Add(res.d.IdItem);
                            }

                            if (removeExistsValue.Count > 0)
                            {
                                var removeTexts = new Dictionary<int, string>();
                                foreach (var pair in removeExistsValue)
                                    removeTexts[pair.Key] = " - №" + pair.Key + " \"" + pair.Value + "\" у объектов №" + string.Join(", №", removeExistsObjects[pair.Key]);

                                throw new Exception("Некоторые значения, которые вы пытаетесь удалить, указаны у следующих объектов:\r\n" + string.Join(", \r\n", removeTexts.Values));
                            }

                            //todo подозрение, что неправильный запрос, может очистить всю таблицу.
                            //DB.CustomFieldsValues.Where(x => !keys.Contains(x.IdFieldValue)).Delete();
                        }

                        result.Message = "Сохранение поля прошло успешно!";
                        result.Success = true;
                        result.Data = data;

                        scope.Commit();
                    }

                    result.Success = true;
                }
            }
            catch (OnUtils.Data.Validation.EntityValidationException ex) { result.Message = ex.CreateComplexMessage(); }
            catch (Exception ex) { result.Message = ex.Message; }

            return ReturnJson(result);
        }

        [ModuleAction("fieldDelete")]
        public JsonResult FieldDelete(int idField = 0)
        {
            var result = JsonAnswer();

            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var data = db.CustomFieldsFields.Where(x => x.IdField == idField).Include(x => x.data).FirstOrDefault();
                    if (data == null) throw new Exception("Такое поле не найдено в базе данных!");

                    using (var scope = db.CreateScope())
                    {
                        db.DeleteEntity(data);
                        if (db.SaveChanges<CustomFieldsField>() > 0)
                        {
                            db.CustomFieldsValues.Where(x => x.IdField == data.IdField).Delete();
                            db.CustomFieldsDatas.Where(x => x.IdField == data.IdField).Delete();

                            result.Message = "Удаление поля прошло успешно.";
                            result.Success = true;

                            scope.Commit();
                        }
                        else throw new Exception("Не получилось удалить поле.");
                    }
                }
            }
            catch (Exception ex) { result.Message = ex.Message; }

            return ReturnJson(result);
        }
    }
}
