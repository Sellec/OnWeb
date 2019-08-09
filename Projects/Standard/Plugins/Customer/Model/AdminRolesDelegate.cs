using OnUtils.Application.DB;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OnWeb.Modules.Customer.Model
{
    using Core.DB;

    public class AdminRolesDelegate
    {
        public AdminRolesDelegate() { }

        public IEnumerable<Role> Roles;
        public IEnumerable<User> Users;
        public Dictionary<int, List<int>> RolesUser;

        public IEnumerable<SelectListItem> ModulesPermissions;

        //public Dictionary<int, int[]> Roles { get; set; }
    }
}