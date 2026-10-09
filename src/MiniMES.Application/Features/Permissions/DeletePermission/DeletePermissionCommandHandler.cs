using PermissionCodes = MiniMES.Application.Authentication.Constrants.Permissions;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Permissions.DeletePermission;

public sealed class DeletePermissionCommandHandler(IApplicationDbContext db) : IRequestHandler<DeletePermissionCommand, Result>
{
    public async Task<Result> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await db.Permissions.SingleOrDefaultAsync(x => x.Id == request.PermissionId, cancellationToken);
        if (permission is null) return Result.Failure(PermissionErrors.NotFound);
        if (permission.Code == PermissionCodes.System.Admin) return Result.Failure(PermissionErrors.Protected);
        db.RolePermissions.RemoveRange(db.RolePermissions.Where(x => x.PermissionId == permission.Id));
        db.Permissions.Remove(permission);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
