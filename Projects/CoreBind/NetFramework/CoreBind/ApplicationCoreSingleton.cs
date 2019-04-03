using System;

namespace OnWeb.CoreBind
{
    static class ApplicationCoreSingleton
    {
        [Obsolete]
        public static ApplicationCore Instance { get; set; }
    }
}
