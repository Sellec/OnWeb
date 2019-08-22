namespace OnWeb.Core.Users
{
#pragma warning disable CS1591 // todo внести комментарии.
    public enum eUserProfileRequired : int
    {
        Nothing = 0,
        OnlyEmail = 1,
        OnlyPhone = 2,
        EmailOrPhone = 3,
        EmailAndPhone = 4,
    }
}
