using System.Collections.Generic;

using TraceCore.Data;

namespace OnWeb.Plugins.Developing
{
    [ModuleCore("Developing", "Функции для разработчиков")]
    public class Module : ModuleCore2<UnitOfWork<DB.File>>
    {
        internal override void InitModuleImmediately(List<ModuleCoreCandidate> candidatesTypes)
        {
            base.InitModuleImmediately(candidatesTypes);
            this.AutoRegister("dev");
        }
    }
}
