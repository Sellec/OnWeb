using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnWeb.Plugins.Customer.Model
{
    public class AdminRoleEdit : TraceWeb.DB.Role
    {
        public AdminRoleEdit()
        { }

        public AdminRoleEdit(TraceWeb.DB.Role role)
        {
            this.IdRole = role.IdRole;
            this.NameRole = role.NameRole;
            this.IdUserChange = role.IdUserChange;
            this.DateChange = role.DateChange;
            this.IdUserCreate = role.IdUserCreate;
            this.DateCreate = role.DateCreate;

            this.Permissions = new List<string>();
        }

        [Display(Name = "Разрешения модуля")]
        public List<string> Permissions { get; set; }
    }
}