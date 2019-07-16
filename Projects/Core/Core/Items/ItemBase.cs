using OnUtils.Application.Items;
using OnUtils.Items;

namespace OnWeb.Core.Items
{
    using Modules;

    /// <summary>
    /// Базовый класс для сущностей. Предоставляет некоторый набор методов и виртуальных свойств, используемых во многих расширениях и частях движка. 
    /// Некоторые же части движка работают ТОЛЬКО с объектами, унаследованными от ItemBase (например, расширение CustomFields). 
    /// Поддерживает атрибут <see cref="ConstructorInitializerAttribute"/> для методов класса.
    /// </summary>
    public abstract partial class ItemBase : OnUtils.Application.Items.ItemBase<WebApplicationBase>
    {
        public ItemBase()
        {
        }

        public ItemBase(IModuleCore owner) : base((OnUtils.Application.Modules.ModuleCore<WebApplicationBase>)owner)
        {
        }

        internal IModuleCoreInternal OwnerModuleWeb
        {
            get => (IModuleCoreInternal)OwnerModule;
        }
    }


    /// <summary>
    /// Базовый класс для сущностей с привязкой к модулю.
    /// Параметр-тип <typeparam name="TModuleType"/> позволяет беспараметрическому конструктору автоматически 
    /// найти объект модуля (обращением к <see cref="ModulesManager.GetModule{TModule}(bool)"/>) и задать в <see cref="ItemBase.Owner"/>.
    /// </summary>
    public abstract class ItemBase<TModuleType> : ItemBase
        where TModuleType : ModuleCore<TModuleType>
    {
        /// <summary>
        /// </summary>
        public ItemBase() : base(GetModule())
        {
        }

        /// <summary>
        /// </summary>
        public ItemBase(TModuleType module) : base(module)
        {
        }

        private static TModuleType GetModule()
        {
            var module = OnUtils.Application.DeprecatedSingletonInstances.Get<WebApplicationBase>().GetModule<TModuleType>();
            return module;
        }
    }
}
