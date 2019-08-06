using OnUtils.Application.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Core.Modules
{
    using Items;
    using Plugins.Routing;

    /// <summary>
    /// Базовый класс для всех веб-модулей. Обязателен при реализации любых модулей, т.к. при задании привязок в DI проверяется наследование именно от этого класса.
    /// </summary>
    /// <typeparam name="TSelfReference">Должен ссылаться сам на себя.</typeparam>
    public abstract class ModuleCore<TSelfReference> : ModuleCore<WebApplication, TSelfReference>, IModuleCoreInternal, IModuleCore
        where TSelfReference : ModuleCore<TSelfReference>
    {
        /// <summary>
        /// Возвращает ссылку для переданного объекта.
        /// Вызывается в случае, когда для объекта не был найден адрес в системе маршрутизации по ключу <see cref="RoutingConstants.MAINKEY"/>.
        /// </summary>
        /// <returns></returns>
        public virtual Uri GenerateLink(ItemBase item)
        {
            throw new NotImplementedException(string.Format("Метод 'GenerateLink' класса '{0}' не определен в производном классе '{1}'", typeof(ModuleCore<TSelfReference>).FullName, GetType().FullName));
        }

        /// <summary>
        /// Возвращает ссылку для переданного объекта.
        /// Вызывается для объектов, для которых не был найден адрес в системе маршрутизации по ключу <see cref="RoutingConstants.MAINKEY"/>.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyDictionary<ItemBase, Uri> GenerateLinks(IEnumerable<ItemBase> items)
        {
            return items.ToDictionary(x => x, x => GenerateLink(x));
        }

        /// <summary>
        /// Возвращает список идентификатор=название указанного типа для текущего модуля. Например, это может быть список категорий.
        /// </summary>
        /// <param name="IdItemType"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        public virtual Types.NestedCollection GetItems(int IdItemType, params object[] _params)
        {
            return null;
        }
    }
}
