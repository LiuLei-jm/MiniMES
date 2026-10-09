using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Permissions.AssignRolePermissions;

public sealed record AssignRolePermissionsCommand(Guid RoleId, IReadOnlyList<Guid> PermissionIds) : IRequest<Result>, IAuditableRequest
{
    public string AuditAction => "Role.AssignPermissions";
    public string AuditTargetType => "Role";
    public string AuditTargetId => RoleId.ToString();
}
