using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Roles.CreateRole;

public sealed record CreateRoleCommand(string Name) : IRequest<Result<Guid>>, IAuditableRequest
{
    public string AuditAction => "Role.Create";
    public string AuditTargetType => "Role";
    public string? AuditTargetId => Name;
}

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator() => RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
}
