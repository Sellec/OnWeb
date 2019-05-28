using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Core.Items
{
    using OnUtils.Items;
    using OnWeb.Core.ModuleExtensions.ExtensionUrl;

    public abstract partial class ItemBase
    {
        [Newtonsoft.Json.JsonIgnore]
        internal Uri _routingUrlMain = null;

        [Newtonsoft.Json.JsonIgnore]
        internal UrlSourceType _routingUrlMainSourceType = UrlSourceType.None;

        [ConstructorInitializer]
        private void ExtensionUrlInitializer()
        {
            if (_routingUrlMain == null && Owner != null)
            {
                if (OwnerModule.Urls != null)
                    OwnerModule.Urls.RegisterToQuery(this);
            }
            //ExtensionUrlStaticInternal.RegisterToQuery(this);
        }

        //[ConstructorInitializer]
        //private void ExtensionUrlInitializer()
        //{
        //    var IdItemType = ItemTypeAttribute.GetValueFromObject(this);
        //    //lock (SyncRoot)
        //    {
        //        _routingUrlMain = QueryList.GetOrAdd(IdItemType, idItemType => new LazyWithoutExceptionCachingList<ItemBase, Uri>(items =>
        //        {
        //            List<ItemBase> itemsList = null;
        //            lock (SyncRoot)
        //            {
        //                itemsList = items.ToList();
        //                //LazyWithoutExceptionCachingList<ItemBase, Uri> dd;
        //                //QueryList.TryRemove(idItemType, out dd);
        //            }

        //            var result = GetForQuery(idItemType, itemsList);
        //            return result;
        //        })).Attach(this);
        //    }
        //}

        [SavedInContextEvent]
        private void ExtensionUrlSaver()
        {
            //if (_fields != null && Owner != null)
            //{
            //    OwnerModule.Fields.SaveItemFields(this);
            //}

        }

        /// <summary>
        /// Указывает источник, предоставивший значение <see cref="Url"/>.
        /// </summary>
        [NotMapped]
        public UrlSourceType UrlSourceType
        {
            get
            {
                var url = Url; // эта строка нужна для получения значения _routingUrlMainSourceType.
                return _routingUrlMainSourceType;
            }
        }

        /// <summary>
        /// Возвращает url-адрес объекта. 
        /// </summary>
        /// <seealso cref="UrlSourceType"/>
        [NotMapped]
        public virtual Uri Url
        {
            get
            {
                if (_routingUrlMain == null && Owner != null)
                {
                    if (OwnerModule.Urls != null)
                    {
                        OwnerModule.Urls.RegisterToQuery(this);
                        OwnerModule.Urls.CheckDeffered(this.GetType());
                    }
                }

                if (_routingUrlMain == null)
                {
                    _routingUrlMain = _empty;
                    _routingUrlMainSourceType = UrlSourceType.None;
                }

                return _routingUrlMain;
            }
        }

        private static Uri _empty = new Uri("http://empty");

    }
}
