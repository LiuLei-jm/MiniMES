using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Roles.UpdateRole;

public sealed record UpdateRoleCommand(Guid RoleId, string Name) : IRequest<Result>, IAuditableRequest
{
    public string AuditAction => "Role.Update";
    public string AuditTargetType => "Role";
    public string AuditTargetId => RoleId.ToString();
}

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.RoleId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
    }
}
