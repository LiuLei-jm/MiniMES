namespace MiniMES.Application.Features.Users.Models;

public sealed record UserListItem(
    Guid Id,
    string UserName,
    bool IsActive,
    DateTime CreatedTime,
    DateTime? LastLoginAt,
    IReadOnlyList<string> Roles);

public sealed record UserDetails(
    Guid Id,
    string UserName,
    bool IsActive,
    DateTime CreatedTime,
    DateTime? LastLoginAt,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);

public sealed record UserPage(IReadOnlyList<UserListItem> Items, int Total, int Page, int PageSize);
