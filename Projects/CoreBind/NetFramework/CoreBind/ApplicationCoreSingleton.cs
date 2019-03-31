using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.CoreBind
{
    static class ApplicationCoreSingleton
    {
        [Obsolete]
        public static Core.ApplicationCore Instance { get; set; }
    }
}
