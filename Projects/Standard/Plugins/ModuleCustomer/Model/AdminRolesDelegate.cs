using System.Collections.Generic;
using System.Web.Mvc;

namespace OnWeb.Plugins.Customer.Model
{
    public class AdminRolesDelegate
    {
        public AdminRolesDelegate() { }

        public IEnumerable<TraceWeb.DB.Role> Roles;
        public IEnumerable<TraceWeb.DB.User> Users;
        public Dictionary<int, List<int>> RolesUser;

        public IEnumerable<SelectListItem> ModulesPermissions;

        //public Dictionary<int, int[]> Roles { get; set; }
    }
}