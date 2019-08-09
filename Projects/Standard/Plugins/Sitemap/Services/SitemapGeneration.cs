using OnUtils.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Modules.Sitemap.Services
{
    using Core;

    public class SitemapGeneration : CoreComponentBase
    {
        #region Static
        private static SitemapGeneration _instance = null;

        /// <summary>
        /// </summary>
        public static void Execute()
        {
            _instance?.ExecuteInternal();
        }
        #endregion

        #region CoreComponentBase
        /// <summary>
        /// </summary>
        protected override void OnStart()
        {
            _instance = this;
        }

        /// <summary>
        /// </summary>
        protected override void OnStop()
        {
            if (_instance == this) _instance = null;
        }
        #endregion

        #region Execute
        private volatile bool _isExecuting = false;

        private void ExecuteInternal()
        {
            if (_isExecuting) return;
            try
            {
                _isExecuting = true;

                var sitemapProviderTypes = AppCore.GetQueryTypes().Where(x => typeof(ISitemapProvider).IsAssignableFrom(x)).ToList();
                var providerList = sitemapProviderTypes.Select(x =>
                {
                    try
                    {
                        return AppCore.Create<ISitemapProvider>(x);
                    }
                    catch
                    {
                        return null;
                    }
                }).Where(x => x != null).ToList();
                var linksAll = providerList.SelectMany(x => x.GetItems() ?? new List<SitemapItem>()).ToList();

                var module = AppCore.GetModulesManager().GetModule<ModuleSitemap>();

                var code = WebUtils.RazorRenderHelper.RenderView(module, "SitemapXml.cshtml", linksAll);

                var path = System.IO.Path.Combine(OnUtils.LibraryEnumeratorFactory.LibraryDirectory, "sitemap.xml");
                System.IO.File.WriteAllText(path, code);

            }
            finally
            {
                _isExecuting = false;
            }
        }

        /// <summary>
        /// Планирует немедленный запуск сервиса.
        /// </summary>
        public void Run()
        {
            TasksManager.SetTask($"{nameof(SitemapGeneration)}_{DateTime.Now.Ticks}", DateTime.Now.AddSeconds(10), () => Execute());
        }
        #endregion

    }
}