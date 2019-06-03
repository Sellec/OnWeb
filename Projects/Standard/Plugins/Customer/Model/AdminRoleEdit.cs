using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnWeb.Plugins.Customer.Model
{
    using Core.DB;

    [NotMapped]
    public class AdminRoleEdit : Role
    {
        public AdminRoleEdit()
        { }

        public AdminRoleEdit(Role role)
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