using OnUtils.Architecture.AppCore;

namespace OnWeb.Plugins.WebCoreModule
{
    using Core;

    sealed class WebCoreConfigurationChecker : CoreComponentBase, IComponentSingleton, ICritical
    {
        #region CoreComponentBase
        protected override void OnStart()
        {
            AppCore.GetWebCoreModule().RunConfigurationCheck();
        }

        protected override void OnStop()
        {
        }
        #endregion
    }
}
