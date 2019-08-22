using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Modules.WebCoreModule
{
    class BindingConstraint : IBindingConstraintHandler
    {
        void IBindingConstraintHandler.CheckBinding(object sender, BindingConstraintEventArgs e)
        {
            if (e.QueryType == typeof(Core.Modules.IModuleController<WebCoreModule>)) e.SetFailed("Запрещено привязывать контроллеры к модулю ядра.");
        }
    }
}
