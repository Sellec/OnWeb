using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using OnUtils.Application.Modules.Extensions;
using System.Threading.Tasks;
using OnUtils.Application.Items;
using OnUtils.Application.Modules;

namespace OnWeb.Core.Modules.Extensions.ExtensionUrl
{
    using Items;
    using Modules;
    using Modules.Extensions;

#pragma warning disable CS1591 // todo внести комментарии.
    [ModuleExtension("ExtensionUrls")]
    public class ExtensionUrl : ModuleExtension<WebApplicationBase>
    {
        private readonly object _syncRoot = new object();

        public ExtensionUrl()
        {
            Task.Delay(60000).ContinueWith(t => TimerCallback());
        }

        public void PrepareItems<T>(IEnumerable<T> items, int IdItemType = 0) where T : ItemBase
        {
            if (IdItemType == 0) IdItemType = ModulesConstants.ItemType;

            if (items != null && items.Count() > 0)
            {
                //var schemeFull = this.getSchemeFullByItem2(schemeItemID, schemeItemType, schemeID);
                //if (schemeFull != null && schemeFull.Count > 0)
            }
        }

        #region Deffered
        #region Clear links
        private class TimerData
        {
            public DateTime DateCreated { get; } = DateTime.Now;

            public Dictionary<ItemBase, Type> Objects = new Dictionary<ItemBase, Type>();
        }

        private ConcurrentQueue<TimerData> _linksListsQueue = new ConcurrentQueue<TimerData>();

        private TimerData TimerClearDeffered
        {
            get
            {
                TimerData data;
                if (!_linksListsQueue.IsEmpty)
                {
                    var last = _linksListsQueue.Last();
                    if ((DateTime.Now - last.DateCreated).TotalMinutes >= 1)
                    {
                        data = new TimerData();
                        _linksListsQueue.Enqueue(data);
                        return data;
                    }
                    else return last;
                }
                else
                {
                    data = new TimerData();
                    _linksListsQueue.Enqueue(data);
                    return data;
                }
            }
        }

        private void TimerCallback()
        {
            try
            {
                while (!_linksListsQueue.IsEmpty)
                {
                    if (_linksListsQueue.TryPeek(out var dataPeek))
                    {
                        if ((DateTime.Now - dataPeek.DateCreated).TotalMinutes >= 1)
                        {
                            if (_linksListsQueue.TryDequeue(out var dataDequeued) && object.ReferenceEquals(dataDequeued, dataPeek))
                            {
                                if (dataDequeued.Objects.Count > 0)
                                {
                                    var objectsGroupedByType = dataDequeued.Objects.Where(x => x.Key != null && x.Value != null).GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.ToList());
                                    lock (_syncRoot)
                                    {
                                        foreach (var pair in objectsGroupedByType)
                                        {
                                            var objType = pair.Key;
                                            if (_defferedObjects.TryGetValue(objType, out var list))
                                            {
                                                foreach (var obj in pair.Value)
                                                {
                                                    list.TryRemove(obj.Key, out var value);
                                                }
                                            }
                                        }
                                    }
                                    dataDequeued.Objects.Clear();
                                }
                            }
                        }
                        else break;
                    }
                    else break;
                }
            }
            finally
            {
                Task.Delay(60000).ContinueWith(t => TimerCallback());
            }
        }
        #endregion

        [Newtonsoft.Json.JsonIgnore]
        private ConcurrentDictionary<Type, ConcurrentDictionary<ItemBase, int>> _defferedObjects = new ConcurrentDictionary<Type, ConcurrentDictionary<ItemBase, int>>();

        public void RegisterToQuery<TItem>(TItem obj) where TItem : ItemBase
        {
            try
            {
                var objType = obj.GetType();
                var list = _defferedObjects.GetOrAdd(objType, t => new ConcurrentDictionary<ItemBase, int>());

                list[obj] = 0;
                TimerClearDeffered.Objects[obj] = objType;
            }
            catch { }
        }

        internal void CheckDeffered(Type type = null)
        {
            if (_defferedObjects.ContainsKey(type))
            {
                Dictionary<ItemBase, int> items = null;
                lock (_syncRoot)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            var newCollection = new ConcurrentDictionary<ItemBase, int>();
                            ConcurrentDictionary<ItemBase, int> oldCollection = null;

                            _defferedObjects.AddOrUpdate(type, newCollection, (key, old) =>
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

        internal Dictionary<ItemBase, Tuple<Uri, UrlSourceType>> GetForQuery(int IdItemType, Type type, IEnumerable<ItemBase> items)
        {
            var absoluteUrl = AppCore.ServerUrl;
            var urlManager = AppCore.Get<Routing.UrlManager>();
            var itemsSet = items.GroupBy(x => x.OwnerModuleWeb, x => x).SelectMany(gr_ =>
            {
                var itemsModule = gr_.ToDictionary<ItemBase, ItemBase, Tuple<Uri, UrlSourceType>>(x => x, x => null);

                if (gr_.Key != null)
                {
                    var keys = itemsModule.Keys.ToList();
                    var result = urlManager.GetUrl(gr_.Key, keys.Select(x => x.ID), IdItemType, Routing.RoutingConstants.MAINKEY);
                    //var result = GetUrl(gr_.Key, keys.Select(x => x.ID), IdItemType);
                    if (!result.IsSuccess)
                    {
                        Debug.WriteLine("ItemBase.GetForQuery({0}): {1}", IdItemType, result.Message);
                        throw new Exception("Ошибка получения адресов");
                    }
                    else
                    {
                        var itemsEmpty = new System.Collections.ObjectModel.Collection<ItemBase>();

                        foreach (var x in keys)
                        {
                            if (result.Result.TryGetValue(x.ID, out string value) && !string.IsNullOrEmpty(value))
                            {
                                if (Uri.TryCreate(value, UriKind.Absolute, out Uri url)) itemsModule[x] = new Tuple<Uri, UrlSourceType>(url, UrlSourceType.Routing);
                                else if (Uri.TryCreate(absoluteUrl, value, out Uri url2)) itemsModule[x] = new Tuple<Uri, UrlSourceType>(url2, UrlSourceType.Routing);
                            }
                            else itemsEmpty.Add(x);
                        }

                        if (itemsEmpty.Count > 0)
                        {
                            var generated = gr_.Key.GenerateLinks(itemsEmpty);
                            if (generated != null)
                                foreach (var pair in generated)
                                    if (pair.Value != null)
                                        itemsModule[pair.Key] = new Tuple<Uri, UrlSourceType>(pair.Value, UrlSourceType.Module);
                        }
                    }
                }
                return itemsModule;
            });

            var measure = new MeasureTime();
            var itemsResult = itemsSet.ToDictionary(x => x.Key, x => x.Value);

            return itemsResult;
        }
        #endregion

    }
}
