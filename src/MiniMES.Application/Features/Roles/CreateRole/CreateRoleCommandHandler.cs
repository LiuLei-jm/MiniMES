using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Domain.Entities;

namespace MiniMES.Application.Features.Roles.CreateRole;

public sealed class CreateRoleCommandHandler(IApplicationDbContext db) : IRequestHandler<CreateRoleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await db.Roles.AnyAsync(x => x.Name == name, cancellationToken)) return Result<Guid>.Failure(RoleErrors.AlreadyExists);
        var role = new Role(name);
        db.Roles.Add(role);
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(role.Id);
    }
}
