using System;
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
    public class ModuleStandard : ModuleCustomer, IMenuProvider
    {
        public static NestedLinkSimple RelativeToModule(string url, string caption, IModuleCore module)
        {
            var moduleAdmin = module.GetAppCore().Get<Admin.ModuleAdmin>();
            return new NestedLinkSimple(caption, new Uri($"/{moduleAdmin.UrlName}/mnadmin/{module.UrlName}/{url}", UriKind.Relative));
        }

        NestedLinkCollection IMenuProvider.GetAdminMenuItemsBase()
        {
            var items = new NestedLinkCollection(
                RelativeToModule("users", "Пользователи", this),
                RelativeToModule("users_add", "Добавить пользователя", this),
                RelativeToModule("customer_modules", "Настройка модулей для ЛК", this),
                RelativeToModule("history", "История", this),
                RelativeToModule("historyUserLog", "История авторизации", this)
            );

            var gr = new NestedLinkGroup(new NestedLinkSimple("Заявки на регистрацию"), RelativeToModule("users/3", "Отклоненные заявки", this));
            items.Add(gr);

            using (var db = new CoreContext())
            {
                var countNewUsers = db.Users.Where(x => x.State == Core.DB.UserState.RegisterWaitForModerate).Count();
                if (countNewUsers > 0) gr.Links.Add(RelativeToModule("users/2", $"Заявки на регистрацию ({countNewUsers})", this));
            }

            items.Add(new NestedLinkGroup("Роли и пользователи",
                    RelativeToModule("rolesManage", "Настройка ролей", this),
                    RelativeToModule("rolesDelegate", "Назначение ролей", this)
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
