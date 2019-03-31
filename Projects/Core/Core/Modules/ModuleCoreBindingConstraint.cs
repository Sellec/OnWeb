using OnUtils.Application.Modules;
using OnUtils.Architecture.AppCore.DI;
using System.Reflection;

namespace OnWeb.Core.Modules
{
    class ModuleCoreBindingConstraint : IBindingConstraintHandler
    {
        void IBindingConstraintHandler.CheckBinding(object sender, BindingConstraintEventArgs e)
        {
            if (typeof(ModuleBase<ApplicationCore>).IsAssignableFrom(e.QueryType) && !typeof(ModuleCore).IsAssignableFrom(e.QueryType))
            {
                e.SetFailed($"Ядро веб-приложения поддерживает только модули, наследующиеся от '{typeof(ModuleCore).FullName}'.");
                return;
            }

            if (typeof(ModuleBase<ApplicationCore>).IsAssignableFrom(e.QueryType))
            {
                var moduleCoreAttribute = e.QueryType.GetCustomAttribute<ModuleCoreAttribute>();
                if (moduleCoreAttribute == null)
                {
                    e.SetFailed($"Тип, наследующий от '{typeof(ModuleCore).FullName}', считается модулем и должен обладать атрибутом '{typeof(ModuleCoreAttribute).FullName}'.");
                    return;
                }
            }
        }
    }
}
