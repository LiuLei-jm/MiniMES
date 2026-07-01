namespace MiniMES.Api.Endpoints.Identity.Login;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = "";
    public DateTime ExpiresAt { get; init; }
}
