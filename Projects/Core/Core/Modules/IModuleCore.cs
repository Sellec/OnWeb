using OnUtils.Application.DB;
using OnUtils.Application.Modules;
using OnUtils.Application.Modules.Extensions;
using OnUtils.Application.Modules.Extensions.CustomFields;
using OnUtils.Application.Users;
using System;
using System.Collections.Generic;

namespace OnWeb.Core.Modules
{
    using Types;

    /// <summary>
    /// Представляет веб-модуль для доступа к нетипизированным функциям модулей без указания TSelfReference.
    /// </summary>
    public interface IModuleCore : IComponentSingleton
    {
        #region Свойства
        /// <summary>
        /// См. <see cref="OnUtils.Application.Modules.ModuleCore{TAppCoreSelfReference}.IdModule"/>.
        /// </summary>
        int IdModule { get; }

        /// <summary>
        /// См. <see cref="OnUtils.Application.Modules.ModuleCore{TAppCoreSelfReference}.Caption"/>.
        /// </summary>
        string Caption { get; }

        /// <summary>
        /// См. <see cref="OnUtils.Application.Modules.ModuleCore{TAppCoreSelfReference}.QueryType"/>.
        /// </summary>
        Type QueryType { get; }

        /// <summary>
        /// Возвращает уникальное имя модуля на основе <see cref="Type.FullName"/> query-типа модуля.
        /// </summary>
        Guid UniqueName { get; }

        /// <summary>
        /// См. <see cref="OnUtils.Application.Modules.ModuleCore{TAppCoreSelfReference}.UrlName"/>.
        /// </summary>
        string UrlName { get; }
        #endregion

        #region Разрешения
        /// <summary>
        /// Проверяет, доступно ли указанное разрешение <paramref name="key"/> пользователю, ассоциированному с текущим контекстом (см. <see cref="UserContextManager{TApplication}.GetCurrentUserContext"/>).
        /// </summary>
        /// <param name="key">Уникальный ключ разрешения. См. <see cref="Permission.Key"/>.</param>
        /// <returns>Возвращает результат проверки.</returns>
        CheckPermissionResult CheckPermission(string key);

        /// <summary>
        /// Проверяет, доступно ли указанное разрешение <paramref name="key"/> пользователю, ассоциированному с контекстом <paramref name="context"/>.
        /// </summary>
        /// <param name="context">Контекст пользователя.</param>
        /// <param name="key">Уникальный ключ разрешения. См. <see cref="Permission.Key"/>.</param>
        /// <returns>Возвращает результат проверки.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если context равен null.</exception>
        CheckPermissionResult CheckPermission(IUserContext context, string key);

        /// <summary>
        /// Проверяет, доступно ли указанное разрешение <paramref name="key"/> пользователю, ассоциированному с текущим контекстом (см. <see cref="UserContextManager{TApplication}.GetCurrentUserContext"/>).
        /// </summary>
        /// <param name="key">Уникальный ключ разрешения. См. <see cref="Permission.Key"/>.</param>
        /// <returns>Возвращает результат проверки.</returns>
        CheckPermissionResult CheckPermission(Guid key);

        /// <summary>
        /// Проверяет, доступно ли указанное разрешение <paramref name="key"/> пользователю, ассоциированному с контекстом <paramref name="context"/>.
        /// </summary>
        /// <param name="context">Контекст пользователя.</param>
        /// <param name="key">Уникальный ключ разрешения. См. <see cref="Permission.Key"/>.</param>
        /// <returns>Возвращает результат проверки.</returns>
        /// <exception cref="ArgumentNullException">Возникает, если <paramref name="context"/> равен null.</exception>
        CheckPermissionResult CheckPermission(IUserContext context, Guid key);
        #endregion

        /// <summary>
        /// См. <see cref="OnUtils.Application.Modules.ModuleCore{TAppCoreSelfReference}.GetItemTypes"/>.
        /// </summary>
        IEnumerable<ItemType> GetItemTypes();

        /// <summary>
        /// Возвращает список идентификатор=название указанного типа для текущего модуля. Например, это может быть список категорий.
        /// </summary>
        /// <param name="idItemType"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        NestedCollection GetItems(int idItemType, params object[] _params);

        #region Расширения
        /// <summary>
        /// Возвращает список подключенных расширений.
        /// </summary>
        List<ModuleExtension<WebApplicationBase>> GetExtensions();

        /// <summary>
        /// Предоставляет доступ к расширению настраиваемых полей.
        /// </summary>
        ExtensionCustomsFieldsBase<WebApplicationBase> Fields { get; }
        #endregion
    }
}
