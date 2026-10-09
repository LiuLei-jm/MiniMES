using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Roles.DeleteRole;

public sealed class DeleteRoleCommandHandler(IApplicationDbContext db) : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await db.Roles.SingleOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken);
        if (role is null) return Result.Failure(RoleErrors.NotFound);
        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase)) return Result.Failure(RoleErrors.BuiltInRole);
        db.UserRoles.RemoveRange(db.UserRoles.Where(x => x.RoleId == role.Id));
        db.RolePermissions.RemoveRange(db.RolePermissions.Where(x => x.RoleId == role.Id));
        db.Roles.Remove(role);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
