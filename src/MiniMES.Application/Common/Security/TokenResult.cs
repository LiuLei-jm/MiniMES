namespace MiniMES.Application.Common.Security;

public sealed class TokenResult
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; init; }
    public string RefreshToken { get; init; } = "";
    public DateTime RefreshTokenExpiresAt { get; init; }
}