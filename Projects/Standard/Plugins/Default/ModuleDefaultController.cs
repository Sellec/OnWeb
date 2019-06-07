using System;
using System.Web.Mvc;

namespace OnWeb.Plugins.Default
{
    using CoreBind.Modules;

    public class ModuleDefaultController : ModuleControllerUser<ModuleDefault>
    {
        public override ActionResult Index()
        {
            using (var db = new Core.DB.CoreContext())
            using (var scope = db.CreateScope())
            {
                var role = new Core.DB.Role()
                {
                    DateCreate = DateTime.Now.Timestamp(),
                    NameRole = "13123",
                };
                db.Role.Add(role);
                db.SaveChanges();
                AppCore.Get<Core.Users.IUsersManager>().AddRoleUsers(role.IdRole, 19.ToEnumerable());
            }

            //this.RegisterEventWithCode(System.Net.HttpStatusCode.InternalServerError, "1313");
            return this.display("Index.cshtml");
        }

    }
}
