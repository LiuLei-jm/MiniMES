using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Domain.Entities;

namespace MiniMES.Application.Features.Permissions.CreatePermission;

public sealed class CreatePermissionCommandHandler(IApplicationDbContext db) : IRequestHandler<CreatePermissionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToLowerInvariant();
        if (await db.Permissions.AnyAsync(x => x.Code == code, cancellationToken)) return Result<Guid>.Failure(PermissionErrors.AlreadyExists);
        var permission = new Permission(code, request.Description.Trim());
        db.Permissions.Add(permission);
        await db.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(permission.Id);
    }
}
