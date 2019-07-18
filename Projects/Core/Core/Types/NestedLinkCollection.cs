using System;
using System.Collections.Generic;

namespace OnWeb.Core.Types
{
    using Items;
    using Modules.Extensions.ExtensionUrl;

#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Содержимое <see cref="NestedLinkCollection"/> в уплощенной форме - без вложенной иерархии.
    /// </summary>
    public class NestedLinkCollectionSimplified : System.Collections.ObjectModel.Collection<KeyValuePair<IItemBaseUrl, string>>
    {

    }

    /// <summary>
    /// Содержит ссылки и группы ссылок с неограниченной вложенностью.
    /// </summary>
    public class NestedLinkCollection : List<IItemBaseUrl>
    {
        /// <summary>
        /// Инициализирует пустой список.
        /// </summary>
        public NestedLinkCollection()
        { }

        /// <summary>
        /// Инициализирует список со значениями из <paramref name="source"/>.
        /// </summary>
        public NestedLinkCollection(params IItemBaseUrl[] source) : base(source)
        { }

        /// <summary>
        /// Инициализирует список со значениями из <paramref name="source"/>.
        /// </summary>
        public NestedLinkCollection(IEnumerable<IItemBaseUrl> source) : base(source)
        { }

        public NestedLinkCollectionSimplified GetSimplifiedHierarchy(string separator = " -> ")
        {
            var items = new NestedLinkCollectionSimplified();

            Action<string, string, IEnumerable<IItemBaseUrl>> action = null;
            action = (parent, _separator, source) =>
            {
                if (source != null)
                    foreach (var item in source)
                    {
                        if (item is NestedLinkGroup group)
                        {
                            items.Add(new KeyValuePair<IItemBaseUrl, string>(group.SourceItem, parent + item.Caption));
                            action(item.Caption + _separator, _separator, group.Links);
                        }
                        else items.Add(new KeyValuePair<IItemBaseUrl, string>(item, parent + item.Caption));
                    }
            };

            action("", separator, this);

            return items;
        }

        /// <summary>
        /// Возвращает список элементов, отфильтрованных при помощи пользовательского фильтра <paramref name="itemFilter"/>.
        /// </summary>
        public List<IItemBaseUrl> FindNodes(Func<IItemBaseUrl, bool> itemFilter)
        {
            if (itemFilter == null) throw new ArgumentNullException(nameof(itemFilter));

            Action<List<IItemBaseUrl>> action = null;
            var filtered = new List<IItemBaseUrl>();

            action = new Action<List<IItemBaseUrl>>(x =>
            {
                foreach (var item in x)
                {
                    if (itemFilter(item)) filtered.AddIfNotExists(item);
                    if (item is NestedLinkGroup group) action(group.Links);
                }
            });

            action(this);

            return filtered;
        }
    }

    /// <summary>
    /// Коллекция ссылок, при этом сам заголовок группы тоже может быть ссылкой. Например, ссылка на категорию.
    /// </summary>
    public class NestedLinkGroup : ItemBase, IItemBaseUrl
    {
        private IItemBaseUrl _groupItem = null;

        public NestedLinkGroup(string caption, params IItemBaseUrl[] childs) : this(new NestedLinkSimple(caption), childs)
        {
        }

        public NestedLinkGroup(IItemBaseUrl groupItem, params IItemBaseUrl[] childs)
        {
            if (groupItem == null) throw new ArgumentNullException(nameof(groupItem));
            _groupItem = groupItem;
            if (childs != null && childs.Length > 0) Links.AddRange(childs);
        }

        /// <summary>
        /// Вложенные ссылки.
        /// </summary>
        public List<IItemBaseUrl> Links { get; } = new List<IItemBaseUrl>();

        public override int ID
        {
            get => _groupItem.ID;
            set { }
        }

        public override string Caption
        {
            get => _groupItem.Caption;
            set { }
        }

        public Uri Url
        {
            get => _groupItem.Url;
        }

        UrlSourceType IItemBaseUrl.UrlSourceType { get; }

        public IItemBaseUrl SourceItem
        {
            get => _groupItem;
        }
    }

    /// <summary>
    /// Простая ссылка.
    /// </summary>
    public class NestedLinkSimple : ItemBase, IItemBaseUrl
    {
        public NestedLinkSimple(string caption, Uri url = null)
        {
            Caption = caption;
            Url = url;
        }

        public override int ID { get; set; }

        public override string Caption { get; set; }

        public Uri Url { get; }

        UrlSourceType IItemBaseUrl.UrlSourceType { get; }
    }
}



