using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Permissions.UpdatePermission;

public sealed record UpdatePermissionCommand(Guid PermissionId, string Code, string Description) : IRequest<Result>, IAuditableRequest
{
    public string AuditAction => "Permission.Update";
    public string AuditTargetType => "Permission";
    public string AuditTargetId => PermissionId.ToString();
}

public sealed class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
{
    public UpdatePermissionCommandValidator()
    {
        RuleFor(x => x.PermissionId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[a-z0-9]+:[a-z0-9:_-]+$");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
    }
}
