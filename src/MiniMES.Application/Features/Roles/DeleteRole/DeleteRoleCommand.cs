using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Roles.DeleteRole;

public sealed record DeleteRoleCommand(Guid RoleId) : IRequest<Result>, IAuditableRequest
{
    public string AuditAction => "Role.Delete";
    public string AuditTargetType => "Role";
    public string AuditTargetId => RoleId.ToString();
}
