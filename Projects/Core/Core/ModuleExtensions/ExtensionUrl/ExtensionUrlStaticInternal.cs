using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.ModuleExtensions.ExtensionUrl
{
    using Items;

    class ExtensionUrlStaticInternal
    {
        internal static Dictionary<ItemBase, Uri> GetForQuery(int IdItemType, Type type, IEnumerable<ItemBase> items)
        {
            //var m = new MeasureTime();

            //try
            {
                //lock (SyncRoot)
                {
                    var itemsSet = items.GroupBy(x => x.OwnerModule, x => x).SelectMany(gr_ =>
                    {
                        var itemsModule = gr_.ToDictionary<ItemBase, ItemBase, Uri>(x => x, x => null);

                        if (gr_.Key != null)
                        {
                            var keys = itemsModule.Keys.ToList();
                            var result = DeprecatedSingletonInstances.UrlManager.GetUrl(gr_.Key, keys.Select(x => x.ID), IdItemType, Routing.Constants.MAINKEY);
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
                                    if (result.Result.ContainsKey(x.ID) && !string.IsNullOrEmpty(result.Result[x.ID]))
                                    {
                                        if (Uri.TryCreate(result.Result[x.ID], UriKind.Absolute, out Uri url)) itemsModule[x] = url;
                                        // todo else if (Uri.TryCreate(ApplicationCore.Instance.ServerUrl, result.Result[x.ID], out Uri url2)) itemsModule[x] = url2;
                                    }
                                    else itemsEmpty.Add(x);
                                }

                                if (itemsEmpty.Count > 0)
                                {
                                    var generated = gr_.Key.GenerateLinks(itemsEmpty);
                                    if (generated != null)
                                        foreach (var pair in generated)
                                            itemsModule[pair.Key] = pair.Value;
                                }
                            }
                        }
                        return itemsModule;
                    });

                    var measure = new MeasureTime();
                    var itemsResult = itemsSet.ToDictionary(x => x.Key, x => x.Value);

                    return itemsResult;
                }
            }
            //finally { Debug.WriteLineNoLog($"ExtensionUrl.GetForQuery IdItemType={IdItemType}, type='{type.FullName}', count = {items.Count()} with {m.Calculate().TotalMilliseconds}ms."); }
        }
    }
}
