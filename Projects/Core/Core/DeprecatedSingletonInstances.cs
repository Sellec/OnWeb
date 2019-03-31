using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core
{
    internal static class DeprecatedSingletonInstances
    {
        public static Modules.ModulesManager ModulesManager { get; set; }

        public static Routing.UrlManager UrlManager { get; set; }

    }
}
