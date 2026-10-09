using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Permissions.CreatePermission;

public sealed record CreatePermissionCommand(string Code, string Description) : IRequest<Result<Guid>>, IAuditableRequest
{
    public string AuditAction => "Permission.Create";
    public string AuditTargetType => "Permission";
    public string? AuditTargetId => Code;
}

public sealed class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(100).Matches("^[a-z0-9]+:[a-z0-9:_-]+$");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
    }
}
