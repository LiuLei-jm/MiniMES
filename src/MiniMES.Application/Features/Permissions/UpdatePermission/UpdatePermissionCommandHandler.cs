using PermissionCodes = MiniMES.Application.Authentication.Constrants.Permissions;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Permissions.UpdatePermission;

public sealed class UpdatePermissionCommandHandler(IApplicationDbContext db) : IRequestHandler<UpdatePermissionCommand, Result>
{
    public async Task<Result> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
    {
        var permission = await db.Permissions.SingleOrDefaultAsync(x => x.Id == request.PermissionId, cancellationToken);
        if (permission is null) return Result.Failure(PermissionErrors.NotFound);
        if (permission.Code == PermissionCodes.System.Admin) return Result.Failure(PermissionErrors.Protected);
        var code = request.Code.Trim().ToLowerInvariant();
        if (await db.Permissions.AnyAsync(x => x.Id != permission.Id && x.Code == code, cancellationToken)) return Result.Failure(PermissionErrors.AlreadyExists);
        permission.Update(code, request.Description.Trim());
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
