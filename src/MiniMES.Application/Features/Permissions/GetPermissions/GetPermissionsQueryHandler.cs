using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Permissions.Models;

namespace MiniMES.Application.Features.Permissions.GetPermissions;

public sealed class GetPermissionsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetPermissionsQuery, Result<IReadOnlyList<PermissionModel>>>
{
    public async Task<Result<IReadOnlyList<PermissionModel>>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        var counts = await db.RolePermissions.GroupBy(x => x.PermissionId)
            .Select(x => new { PermissionId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.PermissionId, x => x.Count, cancellationToken);
        var permissions = await db.Permissions.AsNoTracking().OrderBy(x => x.Code).ToListAsync(cancellationToken);
        IReadOnlyList<PermissionModel> items = permissions
            .Select(x => new PermissionModel(x.Id, x.Code, x.Description, counts.GetValueOrDefault(x.Id)))
            .ToList();
        return Result<IReadOnlyList<PermissionModel>>.Success(items);
    }
}
