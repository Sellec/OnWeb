using OnUtils.Application.Users;

namespace OnWeb.CoreBind.Users
{
    class UserContextManager : UserContextManager<WebApplicationBase>
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
