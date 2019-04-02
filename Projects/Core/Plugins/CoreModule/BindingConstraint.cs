using OnUtils.Architecture.AppCore.DI;

namespace OnWeb.Plugins.ModuleCore
{
    class BindingConstraint : IBindingConstraintHandler
    {
        void IBindingConstraintHandler.CheckBinding(object sender, BindingConstraintEventArgs e)
        {
            if (e.QueryType == typeof(Core.Modules.IModuleController<Module>)) e.SetFailed("Запрещено привязывать контроллеры к модулю ядра.");
        }
    }
}
