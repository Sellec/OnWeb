using System.Collections.Generic;

namespace OnWeb.Plugins.Sitemap
{
    using Core;

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
        List<SitemapItem> GetItems();
    }
}
