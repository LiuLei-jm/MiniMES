namespace MiniMES.Application.Features.Permissions.Models;

public sealed record PermissionModel(Guid Id, string Code, string Description, int RoleCount);
