using System.Collections.Generic;

namespace OnWeb.Core.Modules.Extensions.ExtensionUrl
{
    using Items;

    /// <summary>
    /// Перечисляет виды источников url-адресов объектов (см. <see cref="ItemBase.Url"/>).
    /// </summary>
    public enum UrlSourceType
    {
        /// <summary>
        /// Используется в случае отсутствия url-адреса (когда <see cref="ItemBase.Url"/> равен null или empty).
        /// </summary>
        None,

        /// <summary>
        /// Адрес из основной таблицы маршрутизации (зарегистрированный через <see cref="Routing.UrlManager"/> с ключом <see cref="Routing.RoutingConstants.MAINKEY"/>).
        /// </summary>
        /// <seealso cref="Routing.UrlManager.Register{TModuleType}(ModuleCore{TModuleType}, IEnumerable{Routing.RegisterItem})"/>
        /// <seealso cref="Routing.UrlManager.Register{TModuleType}(ModuleCore{TModuleType}, int, int, string, IEnumerable{Routing.ActionArgument}, string, string)"/>
        Routing,

        /// <summary>
        /// Адрес, сгенерированный модулем.
        /// </summary>
        /// <seealso cref="ModuleCore{TSelfReference}.GenerateLink(ItemBase)"/>
        /// <seealso cref="ModuleCore{TSelfReference}.GenerateLinks(IEnumerable{ItemBase})"/>
        Module,
    }
}
