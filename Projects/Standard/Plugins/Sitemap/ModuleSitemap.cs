using System;

namespace OnWeb.Plugins.Sitemap
{
    using Core.Modules;
    using Services;

    /// <summary>
    /// Модуль карты сайта.
    /// </summary>
    [ModuleCore("Карта сайта")]
    public class ModuleSitemap : ModuleCore<ModuleSitemap>
    {
        public const string PERM_SITEMAP = "sitemap";

        private SitemapGeneration _sitemapService = null;

        protected override void InitModuleCustom()
        {
            RegisterPermission(PERM_SITEMAP, "Управление картой сайта");
        }

        /// <summary>
        /// </summary>
        protected override void OnModuleStart()
        {
            _sitemapService = new SitemapGeneration();
            _sitemapService.Start(AppCore);
        }

        /// <summary>
        /// </summary>
        protected override void OnModuleStop()
        {
            _sitemapService?.Stop();
            _sitemapService = null;
        }

        /// <summary>
        /// Инициирует немедленный запуск сервиса генерации карты сайта.
        /// </summary>
        public void MarkSitemapGenerationToRun()
        {
            if (_sitemapService == null) throw new InvalidOperationException("Сервис генерации карты сайта недоступен. Возможно, модуль был некорректно инициализирован.");
            _sitemapService.Run();
        }


    }
}