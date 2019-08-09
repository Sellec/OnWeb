using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Core.Items
{
    using OnUtils.Items;
    using global::OnWeb.Modules.Routing;

    public abstract partial class ItemBase
    {
        [Newtonsoft.Json.JsonIgnore]
        internal Uri _routingUrlMain = null;

        [Newtonsoft.Json.JsonIgnore]
        internal UrlSourceType _routingUrlMainSourceType = UrlSourceType.None;

        [ConstructorInitializer]
        private void ExtensionUrlInitializer()
        {
            if (!(this is IItemRouted)) return;

            var moduleLink = ModuleRouting._moduleLink;
            if (_routingUrlMain == null && moduleLink != null)
            {
                moduleLink.RegisterToQuery(this);
            }
        }

        /// <summary>
        /// Указывает источник, предоставивший значение <see cref="UrlBase"/>. Рекомендуется реализовывать свойство <see cref="IItemRouted.UrlSourceType"/> через <see cref="UrlSourceTypeBase"/>.
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
        /// Возвращает url-адрес объекта. Рекомендуется реализовывать свойство <see cref="IItemRouted.Url"/> через <see cref="UrlBase"/>.
        /// </summary>
        /// <seealso cref="UrlSourceType"/>
        [NotMapped]
        protected Uri UrlBase
        {
            get
            {
                if (!(this is IItemRouted)) throw new InvalidOperationException($"Для доступа к расширению адресов необходимо наследовать интерфейс '{typeof(IItemRouted).FullName}'.");

                var moduleLink = ModuleRouting._moduleLink;
                if (_routingUrlMain == null && moduleLink != null)
                {
                    moduleLink.RegisterToQuery(this);
                    moduleLink.CheckDeffered(GetType());
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

