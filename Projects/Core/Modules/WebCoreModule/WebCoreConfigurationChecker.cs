using OnUtils.Architecture.AppCore;

namespace OnWeb.Modules.WebCoreModule
{
    using Core;

    sealed class WebCoreConfigurationChecker : CoreComponentBase, IComponentSingleton, ICritical
    {
        #region CoreComponentBase
        protected override void OnStart()
        {
            AppCore.WebCoreModule.RunConfigurationCheck();
        }

        protected override void OnStop()
        {
        }
        #endregion
    }
}
