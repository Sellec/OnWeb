using System;
using System.Collections.Generic;

namespace OnWeb.Core.Types
{
#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Содержимое <see cref="NestedLinkCollection"/> в уплощенной форме - без вложенной иерархии.
    /// </summary>
    public class NestedListCollectionSimplified : System.Collections.ObjectModel.Collection<KeyValuePair<Items.ItemBase, string>>
    {

    }

    /// <summary>
    /// Содержит ссылки и группы ссылок с неограниченной вложенностью.
    /// </summary>
    public class NestedLinkCollection : List<Items.ItemBase>
    {
        /// <summary>
        /// Инициализирует пустой список.
        /// </summary>
        public NestedLinkCollection()
        { }

        /// <summary>
        /// Инициализирует список со значениями из <paramref name="source"/>.
        /// </summary>
        public NestedLinkCollection(params Items.ItemBase[] source) : base(source)
        { }

        /// <summary>
        /// Инициализирует список со значениями из <paramref name="source"/>.
        /// </summary>
        public NestedLinkCollection(IEnumerable<Items.ItemBase> source) : base(source)
        { }

        public NestedListCollectionSimplified GetSimplifiedHierarchy(string separator = " -> ")
        {
            var items = new NestedListCollectionSimplified();

            Action<string, string, IEnumerable<Items.ItemBase>> action = null;
            action = (parent, _separator, source) =>
            {
                if (source != null)
                    foreach (var item in source)
                    {
                        if (item is NestedLinkGroup)
                        {
                            var group = item as NestedLinkGroup;
                            items.Add(new KeyValuePair<Items.ItemBase, string>(group.SourceItem, parent + item.Caption));
                            action(item.Caption + _separator, _separator, group.Links);
                        }
                        else items.Add(new KeyValuePair<Items.ItemBase, string>(item, parent + item.Caption));
                    }
            };

            action("", separator, this);

            return items;
        }

        /// <summary>
        /// Возвращает список элементов, отфильтрованных при помощи пользовательского фильтра <paramref name="itemFilter"/>.
        /// </summary>
        public List<Items.ItemBase> FindNodes(Func<Items.ItemBase, bool> itemFilter)
        {
            if (itemFilter == null) throw new ArgumentNullException(nameof(itemFilter));

            Action<List<Items.ItemBase>> action = null;
            var filtered = new List<Items.ItemBase>();

            action = new Action<List<Items.ItemBase>>(x =>
            {
                foreach (var item in x)
                {
                    if (itemFilter(item)) filtered.AddIfNotExists(item);
                    if (item is NestedLinkGroup) action((item as NestedLinkGroup).Links);
                }
            });

            action(this);

            return filtered;
        }
    }

    /// <summary>
    /// Коллекция ссылок, при этом сам заголовок группы тоже может быть ссылкой. Например, ссылка на категорию.
    /// </summary>
    public class NestedLinkGroup : Items.ItemBase
    {
        private Items.ItemBase _groupItem = null;

        public NestedLinkGroup(string caption, params Items.ItemBase[] childs) : this(new NestedLinkSimple(caption), childs)
        {
        }

        public NestedLinkGroup(Items.ItemBase groupItem, params Items.ItemBase[] childs)
        {
            if (groupItem == null) throw new ArgumentNullException(nameof(groupItem));
            _groupItem = groupItem;
            if (childs != null && childs.Length > 0) Links.AddRange(childs);
        }

        /// <summary>
        /// Вложенные ссылки.
        /// </summary>
        public List<Items.ItemBase> Links { get; } = new List<Items.ItemBase>();

        public override int ID
        {
            get => _groupItem.ID;
            set => _groupItem.ID = value;
        }

        public override string Caption
        {
            get => _groupItem.Caption;
            set => _groupItem.Caption = value; 
        }

        public override DateTime DateChangeBase
        {
            get => _groupItem.DateChangeBase;
            set => _groupItem.DateChangeBase = value; 
        }

        public override Uri Url
        {
            get => _groupItem.Url;
        }

        public Items.ItemBase SourceItem
        {
            get => _groupItem;
        }
    }

    /// <summary>
    /// Простая ссылка.
    /// </summary>
    public class NestedLinkSimple : Items.ItemBase
    {
        public NestedLinkSimple(string caption, Uri url = null)
        {
            Caption = caption;
            Url = url;
        }

        public override int ID { get; set; }

        public override string Caption { get; set; }

        public override Uri Url { get; }

        public static NestedLinkSimple RelativeToModule(string url, string caption, Modules.ModuleCore module)
        {
            var moduleAdmin = module.AppCore.Get<Plugins.Admin.ModuleAdmin>();
            return new NestedLinkSimple(caption, new Uri($"/{moduleAdmin.UrlName}/mnadmin/{module.UrlName}/{url}", UriKind.Relative));
        }


    }
}



