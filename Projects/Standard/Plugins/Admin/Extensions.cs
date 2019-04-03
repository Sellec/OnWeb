using System;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using System.Collections.Generic;

namespace OnWeb.Plugins
{
    using Core.Types;
    using CoreBind.Routing;

    static class Extensions
    {
        public static Type ControllerUser(this Core.Modules.ModuleCore module)
        {
            return module.ControllerTypes.GetValueOrDefault(ControllerTypeDefault.TypeID);
        }

        public static Type ControllerAdmin(this Core.Modules.ModuleCore module)
        {
            return module.ControllerTypes.GetValueOrDefault(ControllerTypeAdmin.TypeID);
        }

        public static NestedLinkCollection GetAdminMenuItems(this Core.Modules.ModuleCore module)
        {
            var list = new NestedLinkCollection();

            var itemsInternal = (module as AdminForModules.Menu.IMenuProvider)?.GetAdminMenuItemsBase();
            if (itemsInternal != null) list.AddRange(itemsInternal);

            try
            {
                if ((module as Core.Modules.ModuleCore).ControllerAdmin() != null)
                {
                    var moduleAdmin = module.AppCore.Get<Admin.ModuleAdmin>();
                    var methods = module.ControllerAdmin().GetMethods();
                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttributes<AdminForModules.Menu.MenuActionAttribute>(true).FirstOrDefault();
                        if (attr != null)
                        {
                            var values = new RouteValueDictionary();
                            values.Add("controller", module.UrlName);
                            values.Add("action", string.IsNullOrEmpty(attr.Alias) ? method.Name : attr.Alias);

                            var link = new NestedLinkSimple(attr.Caption, new Uri($"/{moduleAdmin.UrlName}/mnadmin/{module.UrlName}/{(string.IsNullOrEmpty(attr.Alias) ? method.Name : attr.Alias)}", UriKind.Relative));
                            list.Add(link);
                        }
                    }
                }

                foreach (var extension in module.GetExtensions())
                {
                    var extLinks = extension.getAdminMenu();
                    if (extLinks != null)
                    {
                        list.AddRange(extLinks);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }

            return list;
        }

    }
}