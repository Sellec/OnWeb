using System.Collections.Generic;

namespace OnWeb.Plugins.Sitemap.Design.Model
{
    /// <summary>
    /// Модель для представления Sitemap.cshtml.
    /// </summary>
    public class Sitemap
    {
        /// <summary>
        /// Список провайдеров объектов для карты сайта.
        /// </summary>
        public List<SitemapProvider> ProviderList { get; set; }
    }

    /// <summary>
    /// Хранит информацию об одном провайдере объектов для карты сайта.
    /// </summary>
    public class SitemapProvider
    {
        /// <summary>
        /// См. <see cref="ISitemapProvider.NameProvider"/>
        /// </summary>
        public string NameProvider { get; set; }

        /// <summary>
        /// Название типа объекта провайдера.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// Указывает, не возникает ли ошибок при создании экземпляра провайдера. В случае ошибки в <see cref="TypeName"/> хранится текст ошибки.
        /// </summary>
        public bool IsCreatedNormally { get; set; }
    }
}