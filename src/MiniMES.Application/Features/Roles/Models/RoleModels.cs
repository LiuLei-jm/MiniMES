namespace MiniMES.Application.Features.Roles.Models;

public sealed record RoleListItem(
    Guid Id,
    string Name,
    int UserCount,
    IReadOnlyList<string> Permissions);
