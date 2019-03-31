using System;
using System.Collections.Generic;
using System.Text;

namespace OnWeb.Core.Addresses
{
    /// <summary>
    /// Обозначает определенный административный уровень адреса.
    /// </summary>
    public enum AddressType : byte
    {
        /// <summary>
        /// Страна.
        /// </summary>
        Country = 0,

        /// <summary>
        /// Область.
        /// </summary>
        Region = 1,

        /// <summary>
        /// Район или городской округ.
        /// </summary>
        District = 2,

        /// <summary>
        /// Населенный пункт (город, село, деревня и т.д.).
        /// </summary>
        City = 3,

        /// <summary>
        /// Улица, микрорайон.
        /// </summary>
        Street = 4,

        /// <summary>
        /// Здание.
        /// </summary>
        Building = 5,

        /// <summary>
        /// Указывает, что это IP-адрес.
        /// </summary>
        IP_Address = 254,
    }
}
