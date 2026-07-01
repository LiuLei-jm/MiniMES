namespace MiniMES.Application.Common.Models;

public sealed class UserPermissionInfo
{
    public List<string> Roles { get; init; } = [];
    public List<string> Permissions { get; init; } = [];
}
