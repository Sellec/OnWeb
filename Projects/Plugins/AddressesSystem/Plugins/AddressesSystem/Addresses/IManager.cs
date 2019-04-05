using OnUtils;
using OnUtils.Architecture.AppCore;
using System.Collections.Generic;

namespace OnWeb.Core.Addresses
{
    using DB;

    using ExecutionResultAddress = ExecutionResult<DB.Address>;

    /// <summary>
    /// Представляет менеджер географических адресов.
    /// </summary>
    public interface IManager : IComponentSingleton<ApplicationCore>
    {
        /// <summary>
        /// На основе геолокации пытается найти адрес по IP-адресу <paramref name="address"/>.
        /// </summary>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionResultAddress"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// В случае успеха может вернуть объект адреса любого административного уровня, в зависимости от возможностей сервиса поиска IP-адресов.
        /// </returns>
        ExecutionResult<Address> GetAddressByIP(System.Net.IPAddress address);

        /// <summary>
        /// Пытается найти адрес по коду КЛАДР <paramref name="kodKladr"/>.
        /// </summary>
        /// <returns>
        /// Возвращает объект <see cref="ExecutionResultAddress"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. 
        /// В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.
        /// В случае успеха может вернуть объект адреса любого административного уровня, в зависимости от возможностей сервиса поиска адресов.
        /// </returns>
        ExecutionResult<Address> GetAddressByKladr(string kodKladr);

        ExecutionResult<Dictionary<DaData.Entities.AddressData, Address>> PrepareAddressDataIntoDB(DaData.Entities.AddressData[] results);

        /// <summary>
        /// Пытается найти указанный адрес <paramref name="address"/>.
        /// Поиск ведется в системе Dadata плюс кешированных данных.
        /// </summary>
        /// <returns>Возвращает null или <see cref="DB.Address"/>, если адрес найден.</returns>
        ExecutionResult<Address> SearchAddress(string address = null);

    }
}
