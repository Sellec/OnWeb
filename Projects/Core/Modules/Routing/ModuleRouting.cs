using OnUtils.Application.Items;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace OnWeb.Modules.Routing
{
    using Core.Items;
    using Core.Modules;
    using DefferedDictionary = ConcurrentDictionary<Type, ConcurrentDictionary<Core.Items.ItemBase, int>>;

    class ThreadInfo
    {
        public int ThreadId { get; set; }

        public object SyncRoot { get; set; }

        public DefferedDictionary Collection { get; set; }

        public DateTime DateCreate { get; set; }

        public DateTime? DateClose { get; set; }
    }

#pragma warning disable CS1591 // todo внести комментарии.
    [ModuleCore("Маршрутизация")]
    public class ModuleRouting : ModuleCore<ModuleRouting>
    {
        internal static ModuleRouting _moduleLink = null;

        private ThreadLocal<ThreadInfo> _defferedObjects;

        public ModuleRouting()
        {
            _defferedObjects = new ThreadLocal<ThreadInfo>(() =>
             {
                 var id = Thread.CurrentThread.ManagedThreadId;
                 var oldSameIdThreads = _defferedObjects.Values.Where(x => !x.DateClose.HasValue && x.ThreadId == id).ToList();
                 if (oldSameIdThreads.Count > 0)
                 {
                     Debug.WriteLine($"TimerCallback: new threadID={id}, found {oldSameIdThreads.Count} old threads with {oldSameIdThreads.Sum(x => x.Collection.Sum(y => y.Value.Count))} items.");
                     oldSameIdThreads.ForEach(pair =>
                     {
                         pair.Collection.Values.ForEach(x => x.Clear());
                         pair.Collection.Clear();
                         pair.DateClose = DateTime.Now;
                     });
                 }

                 return new ThreadInfo()
                 {
                     ThreadId = id,
                     DateCreate = DateTime.Now,
                     Collection = new DefferedDictionary(),
                     SyncRoot = new object()
                 };
             }, true);

            Task.Delay(60000).ContinueWith(t => TimerCallback());
        }

        protected override void InitModuleCustom()
        {
            _moduleLink = this;
        }

        protected override void OnModuleStop()
        {
            if (_moduleLink == this) _moduleLink = null;
        }

        #region Deffered
        #region Clear links
        private void TimerCallback()
        {
            try
            {
                var openedThreads = _defferedObjects.Values.Where(x => !x.DateClose.HasValue).ToList();
                var countClosedThreads = _defferedObjects.Values.Count - openedThreads.Count;
                var sumAll = openedThreads.Sum(x => x.Collection.Sum(y => y.Value.Count));

                Debug.WriteLine($"TimerCallback: {openedThreads.Count} threads have containers with {sumAll} items. {countClosedThreads} threads are closed.");

                var rows = openedThreads.
                    Select(x => new { x.ThreadId, x.Collection, CountAll = x.Collection.Sum(y => y.Value.Count) }).
                    Where(x => x.CountAll > 0).
                    OrderBy(x => x.ThreadId).
                    Select(x => $"Thread-{x.ThreadId}: {x.CountAll} items in {x.Collection.Count} types;");
                rows.ForEach(x => Debug.WriteLine($"TimerCallback: {x}"));
            }
            finally
            {
                Task.Delay(60000).ContinueWith(t => TimerCallback());
            }
        }
        #endregion

        /// <summary>
        /// Для текущего потока обрабатывает все объекты в кеше.
        /// </summary>
        public void PrepareCurrentThreadCache()
        {
            if (_defferedObjects.IsValueCreated)
            {
                var collection = _defferedObjects.Value.Collection;
                collection.ForEach(x => CheckDeffered(x.Key));
            }
        }

        /// <summary>
        /// Для текущего потока очищает кеш объектов.
        /// </summary>
        public void ClearCurrentThreadCache()
        {
            if (_defferedObjects.IsValueCreated)
            {
                var collection = _defferedObjects.Value.Collection;
                collection.Values.ForEach(x => x.Clear());
                collection.Clear();
            }
        }

        public void RegisterToQuery<TItem>(TItem obj) where TItem : ItemBase
        {
            try
            {
                var objType = obj.GetType();
                var list = _defferedObjects.Value.Collection.GetOrAdd(objType, t => new ConcurrentDictionary<ItemBase, int>());
                list[obj] = 0;
            }
            catch { }
        }

        internal void CheckDeffered(Type type)
        {
            if (_defferedObjects.IsValueCreated && _defferedObjects.Value.Collection.ContainsKey(type))
            {
                Dictionary<ItemBase, int> items = null;
                lock (_defferedObjects.Value.SyncRoot)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            var newCollection = new ConcurrentDictionary<ItemBase, int>();
                            ConcurrentDictionary<ItemBase, int> oldCollection = null;

                            _defferedObjects.Value.Collection.AddOrUpdate(type, newCollection, (key, old) =>
                            {
                                oldCollection = old;
                                return newCollection;
                            });

                            if (oldCollection != null)
                            {
                                items = oldCollection.ToDictionary(x => x.Key, x => x.Value);
                                oldCollection.Clear();
                            }
                            break;
                        }
                        catch
                        {
                            if (i == 2) throw;
                        }
                    }
                }

                var IdItemType = ItemTypeAttribute.GetValueFromType(type);
                var dataForItems = GetForQuery(IdItemType, type, items.Keys);
                try
                {
                    if (dataForItems != null)
                    {
                        foreach (var p in dataForItems)
                        {
                            p.Key._routingUrlMain = p.Value?.Item1;
                            p.Key._routingUrlMainSourceType = p.Value?.Item2 ?? UrlSourceType.None;
                        }
                    }
                }
                finally { if (dataForItems != null) dataForItems.Clear(); }
            }
        }

        internal Dictionary<ItemBase, Tuple<Uri, UrlSourceType>> GetForQuery(int idItemType, Type type, IEnumerable<ItemBase> items)
        {
            var absoluteUrl = AppCore.ServerUrl;
            var urlManager = AppCore.Get<UrlManager>();
            var itemsSet = items.ToDictionary<ItemBase, ItemBase, Tuple<Uri, UrlSourceType>>(x => x, x => null);

            var keys = itemsSet.Keys.ToList();
            var result = urlManager.GetUrl(keys.Select(x => x.ID), idItemType, RoutingConstants.MAINKEY);
            if (!result.IsSuccess)
            {
                result = urlManager.GetUrl(keys.Select(x => x.ID), idItemType, RoutingConstants.MAINKEY);
            }
            if (!result.IsSuccess)
            {
                Debug.WriteLine("ItemBase.GetForQuery({0}): {1}", idItemType, result.Message);
                throw new Exception("Ошибка получения адресов");
            }

            var itemsEmpty = new System.Collections.ObjectModel.Collection<ItemBase>();

            foreach (var x in keys)
            {
                if (result.Result.TryGetValue(x.ID, out string value) && !string.IsNullOrEmpty(value))
                {
                    if (Uri.TryCreate(value, UriKind.Absolute, out Uri url)) itemsSet[x] = new Tuple<Uri, UrlSourceType>(url, UrlSourceType.Routing);
                    else if (Uri.TryCreate(absoluteUrl, value, out Uri url2)) itemsSet[x] = new Tuple<Uri, UrlSourceType>(url2, UrlSourceType.Routing);
                }
                else itemsEmpty.Add(x);
            }

            if (itemsEmpty.Count > 0)
            {
                itemsEmpty.GroupBy(x => x.OwnerModuleWeb, x => x).ForEach(gr_ =>
                {
                    if (gr_.Key != null)
                    {
                        var itemsModule = gr_.ToList();

                        var generated = gr_.Key.GenerateLinks(itemsModule);
                        if (generated != null)
                            foreach (var pair in generated)
                                if (pair.Value != null)
                                    itemsSet[pair.Key] = new Tuple<Uri, UrlSourceType>(pair.Value, UrlSourceType.Module);
                    }
                });
            }

            return itemsSet;
        }
        #endregion

    }
}
