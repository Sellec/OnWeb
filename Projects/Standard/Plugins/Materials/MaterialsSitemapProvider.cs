using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnUtils.Architecture.AppCore;

namespace OnWeb.Plugins.Materials
{
    using Core.Items;
    using Adminmain.Services;

    class MaterialsSitemapProvider : CoreComponentBase<ApplicationCore>, ISitemapProvider
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

        IEnumerable<ItemBase> ISitemapProvider.GetItems()
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

                var items = news.OfType<ItemBase>().Union(pages.OfType<ItemBase>());

                items.ForEach(item => item.Owner = moduleMaterials);

                return items;
            }
        }
        #endregion
    }
}