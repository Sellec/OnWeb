using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OnWeb.Plugins.Customer.Model
{
    using Core.DB;

    public class AdminUserEdit
    {
        public AdminUserEdit()
        {

        }

        [ScaffoldColumn(false)]
        public IList<Journal> history;

        public User User { get; set; }

        [Display(Name = "Роли пользователя")]
        public IEnumerable<int> UserRoles { get; set; }

        public IEnumerable<SelectListItem> Roles;
    }
}