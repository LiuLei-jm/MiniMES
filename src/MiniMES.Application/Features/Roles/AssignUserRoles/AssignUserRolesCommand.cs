using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Roles.AssignUserRoles;

public sealed record AssignUserRolesCommand(Guid UserId, IReadOnlyList<Guid> RoleIds) : IRequest<Result>, IAuditableRequest
{
    public string AuditAction => "User.AssignRoles";
    public string AuditTargetType => "User";
    public string AuditTargetId => UserId.ToString();
}
