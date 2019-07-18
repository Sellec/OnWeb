using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Core.Items
{
    using Modules.Extensions.ExtensionUrl;
    using OnUtils.Items;

    public abstract partial class ItemBase
    {
        [Newtonsoft.Json.JsonIgnore]
        internal Uri _routingUrlMain = null;

        [Newtonsoft.Json.JsonIgnore]
        internal UrlSourceType _routingUrlMainSourceType = UrlSourceType.None;

        [ConstructorInitializer]
        private void ExtensionUrlInitializer()
        {
            if (!(this is IItemBaseUrl)) return;

            if (_routingUrlMain == null && OwnerModuleWeb != null)
            {
                if (OwnerModuleWeb.Urls != null)
                    OwnerModuleWeb.Urls.RegisterToQuery(this);
            }
        }

        /// <summary>
        /// Указывает источник, предоставивший значение <see cref="Url"/>. Рекомендуется реализовывать свойство <see cref="IItemBaseUrl.UrlSourceType"/> через <see cref="UrlSourceTypeBase"/>.
        /// </summary>
        [NotMapped]
        protected UrlSourceType UrlSourceTypeBase
        {
            get
            {
                var url = UrlBase; // эта строка нужна для получения значения _routingUrlMainSourceType.
                return _routingUrlMainSourceType;
            }
        }

        /// <summary>
        /// Возвращает url-адрес объекта. Рекомендуется реализовывать свойство <see cref="IItemBaseUrl.Url"/> через <see cref="UrlBase"/>.
        /// </summary>
        /// <seealso cref="UrlSourceType"/>
        [NotMapped]
        protected Uri UrlBase
        {
            get
            {
                if (!(this is IItemBaseUrl)) throw new InvalidOperationException($"Для доступа к расширению адресов необходимо наследовать интерфейс '{typeof(IItemBaseUrl).FullName}'.");

                if (_routingUrlMain == null && OwnerModuleWeb != null)
                {
                    if (OwnerModuleWeb.Urls != null)
                    {
                        OwnerModuleWeb.Urls.RegisterToQuery(this);
                        OwnerModuleWeb.Urls.CheckDeffered(GetType());
                    }
                }

                if (_routingUrlMain == null)
                {
                    _routingUrlMain = _empty;
                    _routingUrlMainSourceType = UrlSourceType.None;
                }

                return _routingUrlMain;
            }
        }

        private static Uri _empty = new Uri("http://empty");
    }
}
