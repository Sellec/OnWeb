using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Core.Modules.Extensions.ExtensionUrl
{
    using Items;

    /// <summary>
    /// Представляет сущность с URL-адресом.
    /// </summary>
    public interface IItemBaseUrl : IItemBase
    {
        /// <summary>
        /// Возвращает url-адрес объекта. Рекомендуется реализовывать через <see cref="ItemBase.UrlBase"/>
        /// </summary>
        /// <seealso cref="UrlSourceType"/>
        [NotMapped]
        Uri Url { get; }

        /// <summary>
        /// Указывает источник, предоставивший значение <see cref="Url"/>. Рекомендуется реализовывать через <see cref="ItemBase.UrlSourceTypeBase"/>
        /// </summary>
        [NotMapped]
        UrlSourceType UrlSourceType { get; }
    }
}
