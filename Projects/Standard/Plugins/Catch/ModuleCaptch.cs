using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.Web.Mvc;

namespace OnWeb.Plugins.Captch
{
    [ModuleCore("Captch", "Управление капчей")]
    public class ModuleCaptch : ModuleCore
    {
        internal override void InitModuleImmediately(List<ModuleCoreCandidate> candidatesTypes)
        {
            base.InitModuleImmediately(candidatesTypes);
            this.AutoRegister();
        }
    }
}
