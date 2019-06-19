using OnUtils.Application.Users;

namespace OnWeb.CoreBind.Users
{
    class UserContextManager : Core.Users.UserContextManager
    {
        public override void ClearCurrentUserContext()
        {
            base.ClearCurrentUserContext();
        }

        public override void SetCurrentUserContext(IUserContext context)
        {
            base.SetCurrentUserContext(context);
        }
    }
}
