using System;
using System.Collections.Generic;

namespace OnWeb.Plugins.ModuleCustomer
{
    using Core.DB;
    using Core.Items;
    using Core.Modules;

    /// <summary>
    /// Модуль для управления пользователями и личным кабинетом.
    /// </summary>
    [ModuleCore("Личный кабинет", DefaultUrlName = "Customer")]
    public abstract class Module : ModuleCore<Module>
    {
        public const string PERM_MANAGEUSERS = "manage_users";
        public const string PERM_MANAGEROLES = "manage_roles";
        public const string PERM_VIEWHISTORY = "history";

        /// <summary>
        /// См. <see cref="ModuleCore.InitModuleCustom"/>.
        /// </summary>
        protected sealed override void InitModuleCustom()
        {
            base.InitModuleCustom();

            //registerExtensionNeeded<Core.ModuleExtensions.CustomFields.ExtensionCustomsFields>();
            //registerExtensionNeeded<Core.ModuleExtensions.CustomFields.ExtensionCustomsFieldsAdmin>();
            registerExtensionNeeded<Core.ModuleExtensions.ExtensionUrl.ExtensionUrl>();

            RegisterPermission(PERM_MANAGEUSERS, "Управление пользователями");
            RegisterPermission(PERM_MANAGEROLES, "Управление ролями");
            RegisterPermission(PERM_VIEWHISTORY, "Просмотр истории");
        }

        /// <summary>
        /// В классе-наследнике должен регистрировать платформозависимые валидаторы моделей.
        /// </summary>
        protected abstract void RegisterModelValidators();

        /// <summary>
        /// См. <see cref="ModuleCore.GenerateLink(ItemBase)"/>.
        /// </summary>
        public sealed override Uri GenerateLink(ItemBase item)
        {
            if (item is User) return new Uri(AppCore.ServerUrl, $"{UrlName}/user/{item.ID}");
            //else if (item is Register.Model.Register) return new Uri(ApplicationCore.Instance.ServerUrl, $"{Name}/user/{item.ID}");
            //return base.GenerateLink(item);

            return null;
        }

        /// <summary>
        /// См. <see cref="ModuleCore.GetItemTypes"/>.
        /// </summary>
        public sealed override IEnumerable<ItemType> GetItemTypes()
        {
            var type = new ItemType()
            {
                IdItemType = ItemTypeFactory.ItemType.IdItemType,
                NameItemType = "Пользователь",
                UniqueKey = ItemTypeFactory.ItemType.UniqueKey
            };
            return type.ToEnumerable();
        }

        /// <summary>
        /// Проверяет, может ли текущий пользователь редактировать профиль пользователя с идентификатором <paramref name="IdUser"/>. Генерирует исключение с текстом ошибки, если прав недостаточно.
        /// </summary>
        public virtual void CheckPermissionToEditOtherUser(int IdUser)
        {
            var context = AppCore.GetUserContextManager().GetCurrentUserContext();

            if (IdUser == context.GetIdUser()) return;

            if (!context.IsSuperuser) throw new Exception("Недостаточно прав на редактирование других пользователей.");
        }
    }
}
