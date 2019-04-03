using System.Linq;
using System.Web.Mvc;

namespace OnWeb.Plugins.Customer
{
    using AdminForModules.Menu;
    using Core.DB;
    using Core.Modules;
    using Core.Types;

    /// <summary>
    /// См. <see cref="ModuleCustomer"/>.
    /// </summary>
    class ModuleStandard : ModuleCustomer, IMenuProvider
    {
        NestedLinkCollection IMenuProvider.GetAdminMenuItemsBase()
        {
            var items = new NestedLinkCollection(
                NestedLinkSimple.RelativeToModule("users", "Пользователи", this),
                NestedLinkSimple.RelativeToModule("users_add", "Добавить пользователя", this),
                NestedLinkSimple.RelativeToModule("customer_modules", "Настройка модулей для ЛК", this),
                NestedLinkSimple.RelativeToModule("history", "История", this),
                NestedLinkSimple.RelativeToModule("historyUserLog", "История авторизации", this)
            );

            var gr = new NestedLinkGroup(new NestedLinkSimple("Заявки на регистрацию"), NestedLinkSimple.RelativeToModule("users/3", "Отклоненные заявки", this));
            items.Add(gr);

            using (var db = new CoreContext())
            {
                var countNewUsers = db.Users.Where(x => x.State == Core.DB.UserState.RegisterWaitForModerate).Count();
                if (countNewUsers > 0) gr.Links.Add(NestedLinkSimple.RelativeToModule("users/2", $"Заявки на регистрацию ({countNewUsers})", this));
            }

            items.Add(new NestedLinkGroup("Роли и пользователи",
                    NestedLinkSimple.RelativeToModule("rolesManage", "Настройка ролей", this),
                    NestedLinkSimple.RelativeToModule("rolesDelegate", "Назначение ролей", this)
            ));

            return items;
        }

        /// <summary>
        /// См. <see cref="ModuleCustomer.RegisterModelValidators"/>.
        /// </summary>
        protected override void RegisterModelValidators()
        {
            ModelValidatorProviders.Providers.Insert(0, new NetSpecific.NetFramework.CustomerModelValidatorProvider());
        }
    }
}
