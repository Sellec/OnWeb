using OnUtils.Architecture.AppCore;

namespace OnWeb.Core.Modules
{
    class ModulesLoadStarter : CoreComponentBase<ApplicationCore>, IComponentSingleton<ApplicationCore>, IAutoStart
    {
        public ModulesLoadStarter()
        {
        }

        protected override void OnStart()
        {
            ((ModulesManager)AppCore.GetModulesManager()).StartModules();
        }

        protected override void OnStop()
        {
        }
    }
}
