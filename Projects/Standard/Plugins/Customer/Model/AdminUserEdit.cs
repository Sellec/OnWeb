using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OnWeb.Plugins.Customer.Model
{
    public class AdminUserEdit
    {
        public AdminUserEdit()
        {

        }

        [ScaffoldColumn(false)]
        public IList<TraceWeb.DB.SystemHistoryRecord> history;

        public TraceWeb.DB.User User { get; set; }

        [Display(Name = "Роли пользователя")]
        public IEnumerable<int> UserRoles { get; set; }

        public IEnumerable<SelectListItem> Roles;
    }
}