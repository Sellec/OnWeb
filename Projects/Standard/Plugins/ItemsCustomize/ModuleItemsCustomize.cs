using OnUtils.Application.DB;
using OnUtils.Application.Modules.ItemsCustomize;
using OnUtils.Application.Modules.ItemsCustomize.DB;
using OnUtils.Application.Modules.ItemsCustomize.Scheme;
using OnUtils.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Plugins.ItemsCustomize
{
    using AdminForModules.Menu;
    using Core.Modules;
    using Core.Types;

    /// <summary>
    /// Веб-версия модуля <see cref="ModuleItemsCustomize{TAppCoreSelfReference}"/>. Это нужно, чтобы появилась возможность создавать контроллеры для модуля. 
    /// С точки зрения функционала модуля из OnUtils ничего не меняется - веб-версия ничего не переопределяет.
    /// </summary>
    [ModuleCore("Настройка объектов (веб-версия)")]
    class ModuleItemsCustomize : ModuleCore<ModuleItemsCustomize>, IMenuProvider, IUnitOfWorkAccessor<Context>
    {
        NestedLinkCollection IMenuProvider.GetModuleAdminMenuLinks()
        {
            var moduleAdmin = AppCore.Get<Admin.ModuleAdmin>();

            var modules = AppCore.
                GetModulesManager().
                GetModules().
                OfType<IModuleCore>().
                OrderBy(x => x.Caption).
                Select(x => new NestedLinkSimple(x.Caption, new Uri($"/{moduleAdmin.UrlName}/mnadmin/{UrlName}/FieldsList/{x.IdModule}", UriKind.Relative))).
                ToList();

            var collection = new NestedLinkCollection(new NestedLinkGroup("В модулях", modules.ToArray()));
            return collection;
        }

        /// <summary>
        /// Возвращает список схем, зарегистрированных для модуля <typeparamref name="TModule"/>.
        /// </summary>
        public Dictionary<uint, string> GetSchemeList<TModule>()
            where TModule : ModuleCore<TModule>
        {
            var module = AppCore.Get<TModule>();
            return module != null ? GetSchemeList(module.IdModule) : new Dictionary<uint, string>();
        }

        /// <summary>
        /// Возвращает список схем, зарегистрированных для модуля с идентификатором <paramref name="idModule"/>.
        /// </summary>
        public Dictionary<uint, string> GetSchemeList(int idModule)
        {
            var schemes = new Dictionary<uint, string>() { { 0, "По-умолчанию" } };

            using (var db = this.CreateUnitOfWork())
            {
                foreach (var res in (from p in db.CustomFieldsSchemes where p.IdModule == idModule && p.IdScheme > 0 orderby p.NameScheme select p))
                    schemes[(uint)res.IdScheme] = res.NameScheme;
            }

            return schemes;
        }

        /// <summary>
        /// Возвращает список контейнеров схем, зарегистрированных для текущего модуля.
        /// </summary>
        public Dictionary<ItemType, Dictionary<SchemeItem, string>> GetSchemeItemsList(int idModule)
        {
            var itemsGroups = new Dictionary<ItemType, Dictionary<SchemeItem, string>>();
            var module = AppCore.GetModulesManager().GetModule(idModule) as IModuleCore;
            if (module == null) return null;

            foreach (var itemType in module.GetItemTypes())
            {
                var items = new Dictionary<SchemeItem, string>();

                var _itemsPre = module.GetItems(itemType.IdItemType);
                var _items = _itemsPre != null ? _itemsPre.GetSimplifiedHierarchy() : new NestedCollectionSimplified();

                if (_items != null && _items.Count() > 0)
                    foreach (var res in _items)
                        items[new SchemeItem(res.Key.ID, itemType.IdItemType)] = res.Value;
                else
                    items[new SchemeItem(0, itemType.IdItemType)] = "По-умолчанию";

                if (!items.Any(x => x.Key.IdItem == 0 && x.Key.IdItemType == itemType.IdItemType))
                {
                    var items2 = new Dictionary<SchemeItem, string>();
                    items2[new SchemeItem(0, itemType.IdItemType)] = "По-умолчанию";
                    items.ForEach(x => items2.Add(x.Key, x.Value));
                    items = items2;
                }

                itemsGroups[itemType] = items;
            }

            return itemsGroups;
        }
    }
}

