namespace MiniMES.Application.Authentication.Models;

public sealed class UserPermissionInfo
{
    public List<string> Roles { get; init; } = [];
    public List<string> Permissions { get; init; } = [];
}
