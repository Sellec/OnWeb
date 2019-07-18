using OnUtils.Application.Modules;
using OnUtils.Application.Users;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Plugins.Admin
{
    using AdminForModules.Menu;
    using Core.Items;
    using Core.Modules;
    using Core.Types;

    /// <summary>
    /// См. <see cref="ModuleAdmin"/>.
    /// </summary>
    class ModuleStandard : ModuleAdmin
    {
        /// <summary>
        /// См. <see cref="ModuleAdmin.GetAdminMenuList(IUserContext)"/>.
        /// </summary>
        public override Dictionary<IModuleCore, NestedLinkCollection> GetAdminMenuList(IUserContext userContext)
        {
            var modulesList = AppCore.GetModulesManager().GetModules().OfType<IModuleCore>();
            var mods = new Dictionary<IModuleCore, NestedLinkCollection>();
            var mods_errors = new Dictionary<IModuleCore, string>();

            foreach (var module in modulesList)
            {
                if (module.CheckPermission(userContext, ModulesConstants.PermissionManage) != CheckPermissionResult.Allowed)
                {
                    mods_errors[module] = "Недостаточно прав";
                }
                else
                {
                    var links = module.GetAdminMenuItems();
                    if (links == null) links = new NestedLinkCollection();

                    if (links.Count > 0)
                    {
                        if (module.CheckPermission(userContext, ModulesConstants.PermissionManage) == CheckPermissionResult.Allowed)
                        {
                            mods[module] = links;
                        }
                        else
                        {
                            mods_errors[module] = "Недостаточно прав";
                        }
                    }
                }
            }

            var model = new Dictionary<IModuleCore, NestedLinkCollection>();
            mods.Where(x => x.Value.Count > 0).OrderBy(x => x.Key.Caption).ForEach(x => model[x.Key] = x.Value);
            mods_errors.OrderBy(x => x.Key.Caption).ForEach(x => model[x.Key].Add(new NestedLinkSimple(x.Value)));

            return model;
        }
    }
}
