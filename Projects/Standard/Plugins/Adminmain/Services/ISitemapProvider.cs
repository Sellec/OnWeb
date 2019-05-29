using System.Collections.Generic;
using OnUtils.Architecture.AppCore;

namespace OnWeb.Plugins.Adminmain.Services
{
    using Core.Items;

    /// <summary>
    /// Представляет провайдер объектов для создания карты сайта.
    /// </summary>
    public interface ISitemapProvider : IComponentTransient<ApplicationCore>
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
