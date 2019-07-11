using System;

namespace OnWeb.Core
{
    /// <summary>
    /// Будет удалено в будущих версиях.
    /// </summary>
    public static class DeprecatedSingletonInstances
    {
        [Obsolete("Будет удалено в будущих версиях.")]
        public static Routing.UrlManager UrlManager { get; set; }

    }
}
