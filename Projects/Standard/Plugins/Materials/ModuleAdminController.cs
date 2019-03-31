using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Materials
{
    using AdminForModules.Menu;

    /// <summary>
    /// Представляет контроллер для панели управления.
    /// </summary>
    public class ModuleAdminController : AdminForModules.ModuleAdminController<ModuleMaterials, DB.DataLayerContext>
    {
        [MenuAction("Новости")]
        public ActionResult News()
        {
            var showDeleted = Request.Form.GetValues("ShowDeleted")?.Contains("true") ?? false;
            var query = DB.News.AsQueryable();
            if (!showDeleted) query = query.Where(x => !x.Block);

            var model = query.OrderByDescending(x => x.date).ToList();

            return this.display("Admin/NewsList.cshtml", model);
        }

        public ActionResult NewsEdit(int? IdNews = null)
        {
            var success = false;
            var result = "";

            try
            {
                Materials.DB.News data = null;
                if (!IdNews.HasValue || IdNews.Value <= 0) data = new Materials.DB.News();
                else
                {
                    data = DB.News.Where(x => x.id == IdNews.Value).FirstOrDefault();
                    if (data == null) throw new Exception("Указанная новость не найдена.");

                    if (data.Block)
                    {
                        if (UserManager.Instance.isSuperuser) throw new Exception("Указанная новость удалена (сообщение для суперпользователя).");
                        else throw new Exception("Указанная новость не найдена.");
                    }
                }

                return View("Admin/NewsEdit.cshtml", data);
            }
            catch (Exception ex)
            {
                success = false;
                result = ex.Message;
            }

            return ReturnJson(success, result);
        }

        public JsonResult NewsSave(DB.News model = null)
        {
            var answer = JsonAnswer<int>();

            try
            {
                if (ModelState.IsValid)
                {
                    using (var trans = DB.CreateScope())
                    {
                        Materials.DB.News data = null;
                        if (model.id <= 0)
                        {
                            data = new Materials.DB.News() { date = DateTime.Now, user = UserManager.Instance.ID, status = true, Block = false };
                            DB.News.Add(data);
                        }
                        else
                        {
                            data = DB.News.Where(x => x.id == model.id).FirstOrDefault();
                            if (data == null) throw new Exception("Указанная новость не найдена.");

                            if (data.Block)
                            {
                                if (UserManager.Instance.isSuperuser) throw new Exception("Указанная новость удалена (сообщение для суперпользователя).");
                                else throw new Exception("Указанная новость не найдена.");
                            }
                        }

                        data.name = model.name;
                        data.text = model.text;
                        data.short_text = model.short_text;

                        DB.SaveChanges();

                        answer.Data = data.id;

                        var result = UrlManager.Register(
                            Module,
                            data.id,
                            Items.ItemTypeFactory.Instance.GetItemType(typeof(DB.News)).IdItemType,
                            nameof(ModuleController.ViewNews),
                            new List<UrlManager.ActionArgument>() { new UrlManager.ActionArgument() { ArgumentName = "IdNews", ArgumentValue = data.id } },
                            "news/" + UrlManager.Translate(data.name),
                            UrlManager.MAINKEY
                        );
                        if (!result.IsSuccess) throw new Exception(result.Message);

                        answer.FromSuccess(null);
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                answer.FromException(ex);
                Module.RegisterEvent(Journaling.EventType.Error, "Ошибка сохранения новости", "Модель данных, переданная из формы:\r\n" + Newtonsoft.Json.JsonConvert.SerializeObject(model), ex);
            }

            return ReturnJson(answer);
        }

        public ActionResult NewsDelete(int? IdNews = null)
        {
            var success = false;
            var result = "";

            try
            {
                if (!IdNews.HasValue || IdNews.Value <= 0) throw new Exception("Не указан номер новости.");

                var data = DB.News.Where(x => x.id == IdNews.Value).FirstOrDefault();
                if (data == null) throw new Exception("Указанная новость не найдена.");

                if (data.Block)
                {
                    if (UserManager.Instance.isSuperuser) throw new Exception("Указанная новость удалена (сообщение для суперпользователя).");
                    else throw new Exception("Указанная новость не найдена.");
                }

                data.Block = true;
                DB.SaveChanges();

                success = true;
                //result = "Меню было успешно удалено.";
            }
            catch (Exception ex)
            {
                success = false;
                result = ex.Message;
            }

            return ReturnJson(success, result);
        }


    }

}