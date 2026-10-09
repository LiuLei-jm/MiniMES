using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Permissions.DeletePermission;

public sealed record DeletePermissionCommand(Guid PermissionId) : IRequest<Result>, IAuditableRequest
{
    public string AuditAction => "Permission.Delete";
    public string AuditTargetType => "Permission";
    public string AuditTargetId => PermissionId.ToString();
}
