using System;
using System.Web.Mvc;

namespace OnWeb.Plugins.Default
{
    using CoreBind.Modules;

    public class ModuleDefaultController : ModuleControllerUser<ModuleDefault>
    {
        public override ActionResult Index()
        {
            //this.Module.RegisterPermission("asdasd", "1123");

            //using (var db = new Core.DB.CoreContext())
            //using (var scope = db.CreateScope())
            //{
            //    var role = new Core.DB.Role()
            //    {
            //        DateCreate = DateTime.Now.Timestamp(),
            //        NameRole = "13123",
            //    };
            //    db.Role.Add(role);
            //    db.SaveChanges();

            //    var permission = new Core.DB.RolePermission()
            //    {
            //        IdRole = role.IdRole,
            //        IdModule = this.Module.IdModule,
            //        Permission = "asdasd"
            //    };
            //    db.RolePermission.Add(permission);
            //    db.SaveChanges();

            //    AppCore.Get<Core.Users.UsersManager>().AddRoleUsers(role.IdRole, 19.ToEnumerable());
            //    scope.Commit();
            //}

            //AppCore.GetUserContextManager().TryRestorePermissions(AppCore.GetUserContextManager().GetCurrentUserContext());

            //var perm = Module.CheckPermission("asdasd");

            //this.RegisterEventWithCode(System.Net.HttpStatusCode.InternalServerError, "1313");
            return this.display("Index.cshtml");
        }

    }
}
