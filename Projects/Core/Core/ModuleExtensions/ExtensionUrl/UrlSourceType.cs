using System.Collections.Generic;

namespace OnWeb.Core.ModuleExtensions.ExtensionUrl
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
        /// <seealso cref="Routing.UrlManager.Register(Modules.ModuleCore, IEnumerable{Routing.RegisterItem})"/>
        /// <seealso cref="Routing.UrlManager.Register(Modules.ModuleCore, int, int, string, IEnumerable{Routing.ActionArgument}, string, string)"/>
        Routing,

        /// <summary>
        /// Адрес, сгенерированный модулем.
        /// </summary>
        /// <seealso cref="Modules.ModuleCore.GenerateLink(ItemBase)"/>
        /// <seealso cref="Modules.ModuleCore.GenerateLinks(IEnumerable{ItemBase})"/>
        Module,
    }
}
