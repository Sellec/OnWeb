using System;
using System.Collections.Generic;

namespace OnWeb.Types
{
    using Core.Items;

#pragma warning disable CS1591 // todo внести комментарии.
    /// <summary>
    /// Содержимое <see cref="NestedCollection"/> в уплощенной форме - без вложенной иерархии.
    /// </summary>
    public class NestedCollectionSimplified : System.Collections.ObjectModel.Collection<KeyValuePair<ItemBase, string>>
    {

    }

    /// <summary>
    /// Содержит ссылки и группы ссылок с неограниченной вложенностью.
    /// </summary>
    public class NestedCollection : List<ItemBase>
    {
        /// <summary>
        /// Инициализирует пустой список.
        /// </summary>
        public NestedCollection()
        { }

        /// <summary>
        /// Инициализирует список со значениями из <paramref name="source"/>.
        /// </summary>
        public NestedCollection(params ItemBase[] source) : base(source)
        { }

        /// <summary>
        /// Инициализирует список со значениями из <paramref name="source"/>.
        /// </summary>
        public NestedCollection(IEnumerable<ItemBase> source) : base(source)
        { }

        public NestedCollectionSimplified GetSimplifiedHierarchy(string separator = " -> ")
        {
            var items = new NestedCollectionSimplified();

            Action<string, string, IEnumerable<ItemBase>> action = null;
            action = (parent, _separator, source) =>
            {
                if (source != null)
                    foreach (var item in source)
                    {
                        if (item is NestedGroup)
                        {
                            var group = item as NestedGroup;
                            items.Add(new KeyValuePair<ItemBase, string>(group.SourceItem, parent + item.Caption));
                            action(item.Caption + _separator, _separator, group.Links);
                        }
                        else items.Add(new KeyValuePair<ItemBase, string>(item, parent + item.Caption));
                    }
            };

            action("", separator, this);

            return items;
        }

        /// <summary>
        /// Возвращает список элементов, отфильтрованных при помощи пользовательского фильтра <paramref name="itemFilter"/>.
        /// </summary>
        public List<ItemBase> FindNodes(Func<ItemBase, bool> itemFilter)
        {
            if (itemFilter == null) throw new ArgumentNullException(nameof(itemFilter));

            Action<List<ItemBase>> action = null;
            var filtered = new List<ItemBase>();

            action = new Action<List<ItemBase>>(x =>
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
    public class NestedGroup : ItemBase
    {
        private ItemBase _groupItem = null;

        public NestedGroup(int id, string caption, params ItemBase[] childs) : this(new NestedSimple(id, caption), childs)
        {
        }

        public NestedGroup(ItemBase groupItem, params ItemBase[] childs)
        {
            if (groupItem == null) throw new ArgumentNullException(nameof(groupItem));
            _groupItem = groupItem;
            if (childs != null && childs.Length > 0) Links.AddRange(childs);
        }

        /// <summary>
        /// Вложенные ссылки.
        /// </summary>
        public List<ItemBase> Links { get; } = new List<ItemBase>();

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

        public ItemBase SourceItem
        {
            get => _groupItem;
        }
    }

    /// <summary>
    /// Простая ссылка.
    /// </summary>
    public class NestedSimple : ItemBase
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



