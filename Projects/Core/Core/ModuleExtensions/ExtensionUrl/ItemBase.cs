using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Core.Items
{
    using OnUtils.Items;

    public abstract partial class ItemBase
    {
        [Newtonsoft.Json.JsonIgnore]
        internal Uri _routingUrlMain = null;

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
        /// Возвращает url-адрес объекта, если определен родительский модуль объекта.
        /// </summary>
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

                if (_routingUrlMain == null) _routingUrlMain = _empty;

                return _routingUrlMain;
            }
        }

        private static Uri _empty = new Uri("http://empty");

    }
}
