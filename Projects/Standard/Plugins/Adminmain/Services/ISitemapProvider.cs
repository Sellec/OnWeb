using System.Collections.Generic;

namespace OnWeb.Plugins.Adminmain.Services
{
    using Core;
    using Core.Items;

    /// <summary>
    /// Представляет провайдер объектов для создания карты сайта.
    /// </summary>
    public interface ISitemapProvider : IComponentTransient
    {
        /// <summary>
        /// Название провайдера объектов.
        /// </summary>
        string NameProvider { get; }

        /// <summary>
        /// Возвращает список объектов для карты сайта.
        /// </summary>
        IEnumerable<ItemBase> GetItems();
    }
}
