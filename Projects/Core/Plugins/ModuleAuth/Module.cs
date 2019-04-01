using OnUtils.Data;
using System;
using System.Linq;

namespace OnWeb.Plugins.ModuleAuth
{
    using Core.DB;
    using Core.Modules;

    /// <summary>
    /// Модуль авторизации.
    /// </summary>
    [ModuleCore("Авторизация", DefaultUrlName = "Auth")]
    public class Module : ModuleCore<Module>, IUnitOfWorkAccessor<CoreContext>
    {
        /// <summary>
        /// Указывает, требуется ли на сайте суперпользователь. Если на данный момент активного суперпользователя нет (нет учетки с пометкой суперпользователя или все суперпользователи заблокированы), то 
        /// модуль регистрации пометит ближайшего зарегистрированного пользователя как суперпользователя и сразу сделает активным.
        /// </summary>
        /// <returns></returns>
        public bool IsSuperuserNeeded()
        {
            using (var db = this.CreateUnitOfWork())
            {
                return db.Users.Where(x => x.Superuser != 0 && x.Block == 0 && x.State == UserState.Active).Count() == 0;
            }
        }

        /// <summary>
        /// Указывает, что на сайте нет ни одного пользователя и требуется немедленная регистрация.
        /// </summary>
        /// <returns></returns>
        public bool IsNeededAnyUserToRegister()
        {
            using (var db = this.CreateUnitOfWork())
            {
                return db.Users.Count() == 0;
            }
        }
    }
}
