using OnUtils.Data;
using OnUtils.Tasks;
using System;

namespace OnWeb.Plugins.Adminmain
{
    using Core.Modules;

    /// <summary>
    /// Модуль управления основными функциями.
    /// </summary>
    [ModuleCore(" Система")]
    public class Module : ModuleCore<Module>, IUnitOfWorkAccessor<DataContext>
    {
        public const string PERM_CONFIGMAIN = "configuration_main";
        public const string PERM_MODULES = "modules";
        public const string PERM_SITEMAP = "sitemap";
        public const string PERM_RSS = "rss";
        public const string PERM_ADDRESS = "address_system";
        public const string PERM_ROUTING = "routing_system";
        public const string PERM_MANAGE_MESSAGING = "manage_messaging";

        private Services.SitemapGeneration _sitemapService = null;

        protected override void InitModuleCustom()
        {
            RegisterPermission(PERM_CONFIGMAIN, "Управление настройками");
            RegisterPermission(PERM_MODULES, "Управление модулями");
            RegisterPermission(PERM_SITEMAP, "Управление картой сайта");
            RegisterPermission(PERM_RSS, "Управление фидом RSS");
            RegisterPermission(PERM_ADDRESS, "Система адресов (КЛАДР)");
            RegisterPermission(PERM_ROUTING, "Маршрутизация (ЧПУ)");
            RegisterPermission(PERM_MANAGE_MESSAGING, "Управление рассылками и уведомлениями");
        }

        /// <summary>
        /// </summary>
        protected override void OnModuleStart()
        {
            _sitemapService = new Services.SitemapGeneration();
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
