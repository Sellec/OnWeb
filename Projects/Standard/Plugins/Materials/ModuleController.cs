using System;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Materials
{
    /// <summary>
    /// Представляет контроллер для панели управления.
    /// </summary>
    public class ModuleController : ModuleController<ModuleMaterials, DB.DataLayerContext>
    {
        public ActionResult Index()
        {
            return ViewNewsAll();
        }

        [ModuleAction("newsAll")]
        public ActionResult ViewNewsAll()
        {
            var data = DB.News.Where(x => !x.Block).OrderByDescending(x => x.date).ToList();

            return this.display("NewsList.cshtml", data);
        }

        [ModuleAction("news")]
        public ActionResult ViewNews(int? IdNews = null)
        {
            if (!IdNews.HasValue || IdNews.Value <= 0) throw new Exception("Не указан номер новости.");

            var data = DB.News.Where(x => x.id == IdNews.Value).FirstOrDefault();
            if (data == null) throw new Exception("Указанная новость не найдена.");

            if (data.Block)
            {
                if (UserManager.Instance.isSuperuser) throw new Exception("Указанная новость удалена (сообщение для суперпользователя).");
                else throw new Exception("Указанная новость не найдена.");
            }

            return this.display("News.cshtml", data);
        }


    }

}