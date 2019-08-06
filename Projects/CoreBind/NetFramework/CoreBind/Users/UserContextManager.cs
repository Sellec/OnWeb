using OnUtils.Application.Users;

namespace OnWeb.CoreBind.Users
{
    class UserContextManager : UserContextManager<WebApplication>
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
