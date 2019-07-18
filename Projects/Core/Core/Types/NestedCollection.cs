using System;
using System.Collections.Generic;

namespace OnWeb.Core.Types
{
#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Содержимое <see cref="NestedCollection"/> в уплощенной форме - без вложенной иерархии.
    /// </summary>
    public class NestedCollectionSimplified : System.Collections.ObjectModel.Collection<KeyValuePair<Items.ItemBase, string>>
    {

    }

    /// <summary>
    /// Содержит ссылки и группы ссылок с неограниченной вложенностью.
    /// </summary>
    public class NestedCollection : List<Items.ItemBase>
    {
        /// <summary>
        /// Инициализирует пустой список.
        /// </summary>
        public NestedCollection()
        { }

        /// <summary>
        /// Инициализирует список со значениями из <paramref name="source"/>.
        /// </summary>
        public NestedCollection(params Items.ItemBase[] source) : base(source)
        { }

        /// <summary>
        /// Инициализирует список со значениями из <paramref name="source"/>.
        /// </summary>
        public NestedCollection(IEnumerable<Items.ItemBase> source) : base(source)
        { }

        public NestedCollectionSimplified GetSimplifiedHierarchy(string separator = " -> ")
        {
            var items = new NestedCollectionSimplified();

            Action<string, string, IEnumerable<Items.ItemBase>> action = null;
            action = (parent, _separator, source) =>
            {
                if (source != null)
                    foreach (var item in source)
                    {
                        if (item is NestedGroup)
                        {
                            var group = item as NestedGroup;
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
                    if (item is NestedGroup) action((item as NestedGroup).Links);
                }
            });

            action(this);

            return filtered;
        }
    }

    /// <summary>
    /// Коллекция ссылок, при этом сам заголовок группы тоже может быть ссылкой. Например, ссылка на категорию.
    /// </summary>
    public class NestedGroup : Items.ItemBase
    {
        private Items.ItemBase _groupItem = null;

        public NestedGroup(int id, string caption, params Items.ItemBase[] childs) : this(new NestedSimple(id, caption), childs)
        {
        }

        public NestedGroup(Items.ItemBase groupItem, params Items.ItemBase[] childs)
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

        public Items.ItemBase SourceItem
        {
            get => _groupItem;
        }
    }

    /// <summary>
    /// Простая ссылка.
    /// </summary>
    public class NestedSimple : Items.ItemBase
    {
        public NestedSimple(int id, string caption)
        {
            ID = id;
            Caption = caption;
        }

        public override int ID { get; set; }

        public override string Caption { get; set; }
    }
}



