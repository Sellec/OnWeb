using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Plugins.Materials
{
    using Sitemap;
    using Core;
    using Core.Items;
    using Core.Modules.Extensions.ExtensionUrl;

    class MaterialsSitemapProvider : CoreComponentBase, ISitemapProvider
    {
        #region CoreComponentBase
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }
        #endregion

        #region ISitemapProvider
        string ISitemapProvider.NameProvider => "Карта материалов и новостей";

        List<SitemapItem> ISitemapProvider.GetItems()
        {
            var moduleMaterials = AppCore.Get<ModuleMaterials>();

            using (var db = new DB.DataLayerContext())
            {
                var news = (from p in db.News
                            orderby p.name ascending
                            where p.status
                            select p).ToList();

                var pages = (from p in db.Pages
                             orderby p.name ascending
                             where p.status > 0
                             select p).ToList();

                var items = news.OfType<ItemBase>().Union(pages.OfType<ItemBase>()).OfType<IItemBaseUrl>();

                //items.ForEach(item => item.OwnerModule = moduleMaterials);

                return items.Select(x => new SitemapItem()
                {
                    Location = x.Url,
                    LastModificationTime = (x as ItemBase).DateChangeBase >= DateTime.MinValue ? (DateTime?)((x as ItemBase).DateChangeBase) : null
                }).ToList();
            }
        }
        #endregion
    }
}