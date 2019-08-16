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

#pragma warning disable CS1591 // todo внести комментарии.
    [ModuleCore("Маршрутизация")]
    public class ModuleRouting : ModuleCore<ModuleRouting>
    {
        internal static ModuleRouting _moduleLink = null;

        private ThreadLocal<object> _syncRoot = new ThreadLocal<object>(() => new object());
        private ThreadLocal<Tuple<int, DefferedDictionary>> _defferedObjects = new ThreadLocal<Tuple<int, DefferedDictionary>>(() => new Tuple<int, DefferedDictionary>(Thread.CurrentThread.ManagedThreadId, new DefferedDictionary()), true);

        public ModuleRouting()
        {
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
                var sumAll = _defferedObjects.Values.Sum(x => x.Item2.Sum(y => y.Value.Count));
                Debug.WriteLine($"TimerCallback: {_defferedObjects.Values.Count} threads have containers with {sumAll} items.");
                var rows = _defferedObjects.Values.OrderBy(x => x.Item1).Select(x => $"Thread-{x.Item1}: {x.Item2.Sum(y => y.Value.Count)} items in {x.Item2.Count} types;");
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
            var collection = _defferedObjects.Value.Item2;
            collection.ForEach(x => CheckDeffered(x.Key));
        }

        /// <summary>
        /// Для текущего потока очищает кеш объектов.
        /// </summary>
        public void ClearCurrentThreadCache()
        {
            var collection = _defferedObjects.Value.Item2;
            collection.Values.ForEach(x => x.Clear());
            collection.Clear();
        }

        public void RegisterToQuery<TItem>(TItem obj) where TItem : ItemBase
        {
            try
            {
                var objType = obj.GetType();
                var list = _defferedObjects.Value.Item2.GetOrAdd(objType, t => new ConcurrentDictionary<ItemBase, int>());
                list[obj] = 0;
            }
            catch { }
        }

        internal void CheckDeffered(Type type)
        {
            if (_defferedObjects.Value.Item2.ContainsKey(type))
            {
                Dictionary<ItemBase, int> items = null;
                lock (_syncRoot.Value)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            var newCollection = new ConcurrentDictionary<ItemBase, int>();
                            ConcurrentDictionary<ItemBase, int> oldCollection = null;

                            _defferedObjects.Value.Item2.AddOrUpdate(type, newCollection, (key, old) =>
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
