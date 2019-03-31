using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnWeb.CoreBind.Modules.Internal
{
    using Core.Modules;

    public class ModuleInternalErrors : ModuleCore<ModuleInternalErrors>
    {
        protected override void InitModuleCustom()
        {
            // todo _moduleCaption = "Ошибка";
            //_moduleID = 0;
            //_moduleName = "error";
            //_moduleNameBase = "Error";

            base.InitModuleCustom();
        }
    }
}
