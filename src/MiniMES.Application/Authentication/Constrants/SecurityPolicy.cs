namespace MiniMES.Application.Authentication.Constrants;

public static class SecurityPolicy
{
    public const int MaxFailedAttempts = 5;
    public const int LockoutMinutes = 15;
}
