using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core
{
    /// <summary>
    /// Будет удалено в будущих версиях.
    /// </summary>
    public static class DeprecatedSingletonInstances
    {
        [Obsolete("Будет удалено в будущих версиях.")]
        public static OnUtils.Application.Modules.ModulesManager<ApplicationCore> ModulesManager { get; set; }

        [Obsolete("Будет удалено в будущих версиях.")]
        public static Routing.UrlManager UrlManager { get; set; }

    }
}
