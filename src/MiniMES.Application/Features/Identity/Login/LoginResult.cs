namespace MiniMES.Application.Features.Identity.Login;

public sealed class LoginResult
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
