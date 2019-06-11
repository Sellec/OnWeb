using OnUtils.Application.Modules;
using System.Collections.Generic;
using System.Linq;

namespace OnWeb.Plugins.Admin
{
    using Core.Items;
    using Core.Modules;
    using Core.Types;
    using OnUtils.Application.Users;

    /// <summary>
    /// См. <see cref="ModuleAdmin"/>.
    /// </summary>
    class ModuleStandard : ModuleAdmin
    {
        /// <summary>
        /// См. <see cref="ModuleAdmin.GetAdminMenuList(IUserContext)"/>.
        /// </summary>
        public override Dictionary<ModuleCore, List<ItemBase>> GetAdminMenuList(IUserContext userContext)
        {
            var modulesList = AppCore.GetModulesManager().GetModules();
            var mods = new Dictionary<ModuleCore, List<ItemBase>>();
            var mods_errors = new Dictionary<ModuleCore, List<ItemBase>>();

            foreach (var module in modulesList)
            {
                if (module.CheckPermission(userContext, ModulesConstants.PermissionManage) != CheckPermissionResult.Allowed)
                {
                    mods_errors.Add(module, new List<ItemBase>() { new NestedLinkSimple("Недостаточно прав") });
                }
                else
                {
                    var links = (module as ModuleCore).GetAdminMenuItems();
                    if (links == null) links = new NestedLinkCollection();

                    if (links.Count > 0)
                    {
                        if (module.CheckPermission(userContext, ModulesConstants.PermissionManage) == CheckPermissionResult.Allowed)
                        {
                            mods.Add(module, links);
                        }
                        else
                        {
                            mods_errors.Add(module, new List<ItemBase>() { new NestedLinkSimple("Недостаточно прав") });
                        }
                    }
                }
            }

            var model = new Dictionary<ModuleCore, List<ItemBase>>();
            mods.Where(x => x.Value.Count > 0).OrderBy(x => x.Key.Caption).ForEach(x => model[x.Key] = x.Value);
            mods_errors.Where(x => x.Value.Count > 0).OrderBy(x => x.Key.Caption).ForEach(x => model[x.Key] = x.Value);

            return model;
        }
    }
}
