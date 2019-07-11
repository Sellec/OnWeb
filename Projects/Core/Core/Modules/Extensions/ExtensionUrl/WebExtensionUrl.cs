using OnUtils;
using OnUtils.Application.Modules;
using System;
using System.Collections.Generic;
using ExtensionUrlType = OnUtils.Application.Modules.Extensions.ExtensionUrl;

namespace OnWeb.Core.Modules.Extensions.ExtensionUrl
{
    class WebExtensionUrl : ExtensionUrlType.ExtensionUrl
    {
        protected override Uri GetBaseUrlForAbsolute()
        {
            return ((WebApplicationBase)AppCore).ServerUrl;
        }

        protected override ExecutionResult<Dictionary<int, string>> GetUrl(ModuleCore module, IEnumerable<int> idItemList, int idItemType)
        {
            return AppCore.Get<Routing.UrlManager>().GetUrl(module, idItemList, idItemType, Routing.RoutingConstants.MAINKEY);
        }
    }
}
