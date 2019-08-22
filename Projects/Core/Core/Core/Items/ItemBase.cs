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
    public abstract partial class ItemBase : ItemBase<WebApplication>
    {
        internal IModuleCoreInternal OwnerModuleWeb
        {
            get => (IModuleCoreInternal)OwnerModule;
        }
    }
}
