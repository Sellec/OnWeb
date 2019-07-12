using OnUtils.Application.Items;
using OnUtils.Application.Modules;
using OnUtils.Application.Modules.Extensions.CustomFields;
using OnUtils.Application.Modules.Extensions.ExtensionUrl;
using OnUtils.Application.Types;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Plugins.Materials
{
    [ModuleCore("Контент", DefaultUrlName = "Content")]
    public class ModuleMaterials : ModuleCore<ModuleMaterials>, IUnitOfWorkAccessor<DB.DataLayerContext>
    {
        protected override void InitModuleCustom()
        {
            RegisterExtension<ExtensionUrl>();
            RegisterExtension<ExtensionCustomsFieldsBase>();
        }

        public override IReadOnlyDictionary<ItemBase, Uri> GenerateLinks(IEnumerable<ItemBase> items)
        {
            var news = items.Where(x => x is DB.News).ToDictionary(x => x, x => new Uri("/" + UrlName + "/news/" + x.ID, UriKind.Relative));
            var pages = items.Where(x => x is DB.Page).ToDictionary(x => x, x => new Uri("/" + UrlName + "/page/" + x.ID, UriKind.Relative));

            return news.Union(pages).ToDictionary(x => x.Key, x => x.Value);
        }

        //public override IList<AdminMenuItem> getAdminMenuItems()
        //{
        //    var items = new List<AdminMenuItem>();
        //    //items.Add(new AdminMenuItemLink("Основные настройки", new Uri("index"));
        //    //items.Add(new AdminMenuItemLink("Основные настройки", "index"));
        //    //items.Add(new AdminMenuItemLink("Основные настройки", "index"));
        //    //items.Add(new AdminMenuItemLink("Основные настройки", "index"));
        //    //items.Add(new AdminMenuItemLink("Основные настройки", "index"));
        //    //items.Add(new AdminMenuItemLink("Основные настройки", "index"));
        //    //items.Add(new AdminMenuItemLink("Основные настройки", "index"));
        //    //items.Add(new AdminMenuItemLink("Основные настройки", "index"));
        //    //items.Add(new AdminMenuItemLink("Основные настройки", "index"));
        //    return items;
        //}

        public IList<DB.Page> getPagesList()
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    return (from p in db.Pages
                            where p.status > 0
                            orderby p.name ascending
                            select p).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.Logs(ex.Message);
                return null;
            }
        }

        public DB.Page getPageByID(int IdPage)
        {
            try
            {
                using (var db = this.CreateUnitOfWork())
                {
                    return (from p in db.Pages where p.id == IdPage select p).FirstOrDefault();
                }
            }
            catch(Exception ex)
            {
                Debug.Logs(ex.Message);
                return null;
            }
        }

        public override NestedLinkCollection GetItems(int IdItemType, params object[] _params)
        {
            if (IdItemType == ItemType)
            {
                using (var db = this.CreateUnitOfWork())
                {
                    var f = (from p in db.Pages
                             orderby p.name ascending
                             select p).ToList();

                    f.ForEach(page => { page.Owner = this; });

                    return new NestedLinkCollection(f);
                    //return base.getItemsList(IdItemType, SortOrder, _params);
                }
            }
            return null;
        }

        public override Uri GenerateLink(ItemBase item)
        {
            if (item.Owner == this && item is DB.Page page) return new Uri(string.Format("/{0}", page.urlname), UriKind.Relative);
            return null;
        }
    }
}
