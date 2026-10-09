using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Domain.Entities;

namespace MiniMES.Application.Features.Permissions.AssignRolePermissions;

public sealed class AssignRolePermissionsCommandHandler(IApplicationDbContext db)
    : IRequestHandler<AssignRolePermissionsCommand, Result>
{
    public async Task<Result> Handle(AssignRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var role = await db.Roles.SingleOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken);
        if (role is null) return Result.Failure(RoleErrors.NotFound);
        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase)) return Result.Failure(RoleErrors.BuiltInRole);
        var permissionIds = request.PermissionIds.Distinct().ToList();
        if (await db.Permissions.CountAsync(x => permissionIds.Contains(x.Id), cancellationToken) != permissionIds.Count)
            return Result.Failure(PermissionErrors.NotFound);
        db.RolePermissions.RemoveRange(db.RolePermissions.Where(x => x.RoleId == role.Id));
        db.RolePermissions.AddRange(permissionIds.Select(id => new RolePermission { RoleId = role.Id, PermissionId = id }));
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
