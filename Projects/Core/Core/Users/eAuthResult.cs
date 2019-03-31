namespace OnWeb.Core.Users
{
#pragma warning disable CS1591 // todo внести комментарии.
    public enum eAuthResult
    {
        Success,
        YetAuthorized,
        MultipleFound,
        NothingFound,
        WrongAuthData,
        WrongPassword,
        AuthDisabled,
        AuthMethodNotAllowed,
        BlockedUntil,
        RegisterNeedConfirmation,
        RegisterWaitForModerate,
        RegisterDecline,
        Disabled,
        UnknownError
    }

}
