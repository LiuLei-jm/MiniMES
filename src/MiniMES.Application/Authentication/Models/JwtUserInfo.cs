namespace MiniMES.Application.Authentication.Models;

public sealed class JwtUserInfo
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = [];
    public List<string> Permissions { get; init; } = [];
}
