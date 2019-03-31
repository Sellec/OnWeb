using OnUtils;
using OnUtils.Architecture.AppCore;
using System.Collections.Generic;

namespace OnWeb.Core.Users
{
    using ExecutionResultEntities = ExecutionResult<IEnumerable<UserEntity>>;

    /// <summary>
    /// Представляет менеджер, управляющий объектами пользователя. 
    /// Используется, например, в случаях, когда необходимо привязать к учетной записи пользователя какой-либо объект (ссылка на избранное, текстовый документ и т. д.).
    /// </summary>
    public interface IEntitiesManager : IComponentSingleton<ApplicationCore>
    {
        /// <summary>
        /// Возвращает список объектов пользователя с типом <paramref name="entityType"/>.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultEntities"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        ExecutionResultEntities GetEntitiesByEntityType(string entityType = null);

        /// <summary>
        /// Возвращает список объектов пользователя с идентификатором <paramref name="idUser"/>. Если параметр <paramref name="entityTag"/> не пуст, то полученные объекты фильтруются по тегу.
        /// </summary>
        /// <returns>Возвращает объект <see cref="ExecutionResultEntities"/> со свойством <see cref="ExecutionResult.IsSuccess"/> в зависимости от успешности выполнения операции. В случае ошибки свойство <see cref="ExecutionResult.Message"/> содержит сообщение об ошибке.</returns>
        ExecutionResultEntities GetUserEntities(int idUser = 0, string entityTag = null);
    }
}
