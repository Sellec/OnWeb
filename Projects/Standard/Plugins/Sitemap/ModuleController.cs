using System;
using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Sitemap
{
    using AdminForModules.Menu;
    using CoreBind.Modules;

    /// <summary>
    /// Представляет контроллер для панели управления.
    /// </summary>
    class ModuleController : ModuleControllerAdmin<ModuleSitemap>
    {
        public override ActionResult Index()
        {
            return Sitemap();
        }

        [MenuAction("Sitemap", "sitemap", ModuleSitemap.PERM_SITEMAP)]
        public ActionResult Sitemap()
        {
            var sitemapProviderTypes = AppCore.GetQueryTypes().Where(x => typeof(ISitemapProvider).IsAssignableFrom(x)).ToList();
            var providerList = sitemapProviderTypes.Select(x =>
            {
                var p = new Design.Model.SitemapProvider()
                {
                    NameProvider = "",
                    TypeName = x.FullName,
                    IsCreatedNormally = false
                };
                try
                {
                    var pp = AppCore.Create<ISitemapProvider>(x);
                    p.NameProvider = pp.NameProvider;
                    p.IsCreatedNormally = true;
                }
                catch (Exception ex)
                {
                    p.TypeName = ex.ToString();
                    p.IsCreatedNormally = false;
                }
                return p;
            }).ToList();

            return View("Sitemap.cshtml", new Design.Model.Sitemap() { ProviderList = providerList });
        }

        [ModuleAction("sitemap_save", ModuleSitemap.PERM_SITEMAP)]
        public JsonResult SitemapGenerate()
        {
            var success = false;
            var result = "";

            try
            {
                Module.MarkSitemapGenerationToRun();

                success = true;
                result = "Процесс обновления карты сайта запущен.";
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