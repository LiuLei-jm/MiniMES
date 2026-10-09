using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Roles.UpdateRole;

public sealed class UpdateRoleCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdateRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await db.Roles.SingleOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken);
        if (role is null) return Result.Failure(RoleErrors.NotFound);
        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase)) return Result.Failure(RoleErrors.BuiltInRole);
        var name = request.Name.Trim();
        if (await db.Roles.AnyAsync(x => x.Id != role.Id && x.Name == name, cancellationToken)) return Result.Failure(RoleErrors.AlreadyExists);
        role.Rename(name);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
