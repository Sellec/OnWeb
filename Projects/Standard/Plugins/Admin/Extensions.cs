using OnUtils.Application.Modules;
using OnUtils.Application.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Routing;

namespace OnWeb.Plugins
{
    using Core.Modules;
    using CoreBind.Routing;
    using Core;
    using Core.Configuration;
    using Core.Items;
    using Core.Types;


    static class Extensions
    {
        public static Type ControllerUser(this IModuleCore module)
        {
            return module.GetAppCore().Get<ModuleControllerTypesManager>().GetModuleControllerTypes(module.QueryType).GetValueOrDefault(ControllerTypeDefault.TypeID);
        }

        public static Type ControllerAdmin(this IModuleCore module)
        {
            return module.GetAppCore().Get<ModuleControllerTypesManager>().GetModuleControllerTypes(module.QueryType).GetValueOrDefault(ControllerTypeAdmin.TypeID);
        }

        public static NestedLinkCollection GetAdminMenuItems(this IModuleCore module)
        {
            var list = new NestedLinkCollection();

            var itemsInternal = (module as AdminForModules.Menu.IMenuProvider)?.GetAdminMenuItemsBase();
            if (itemsInternal != null) list.AddRange(itemsInternal);

            try
            {
                if (module.ControllerAdmin() != null)
                {
                    var moduleAdmin = module.GetAppCore().Get<Admin.ModuleAdmin>();
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
                    if (extension is Core.Modules.Extensions.CustomFields.ExtensionCustomsFieldsAdmin fields)
                    {
                        var extLinks = fields.GetAdminMenu();
                        if (extLinks != null)
                        {
                            list.AddRange(extLinks);
                        }
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