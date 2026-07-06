namespace MiniMES.Application.Authentication.Models;

public sealed class AuthenticationResult
{
    public string AccessToken { get; init; } = "";
    public string RefreshToken { get; init; } = "";
    public DateTime ExpiresAt { get; init; }
}
