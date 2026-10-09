using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Roles.Models;

namespace MiniMES.Application.Features.Roles.GetRoles;

public sealed class GetRolesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetRolesQuery, Result<IReadOnlyList<RoleListItem>>>
{
    public async Task<Result<IReadOnlyList<RoleListItem>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await db.Roles.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        var userCounts = await db.UserRoles.GroupBy(x => x.RoleId)
            .Select(x => new { RoleId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.RoleId, x => x.Count, cancellationToken);
        var permissionRows = await (from rolePermission in db.RolePermissions
                                    join permission in db.Permissions on rolePermission.PermissionId equals permission.Id
                                    select new { rolePermission.RoleId, permission.Code })
            .AsNoTracking().ToListAsync(cancellationToken);
        var permissions = permissionRows.GroupBy(x => x.RoleId)
            .ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Select(y => y.Code).Order().ToList());
        IReadOnlyList<RoleListItem> items = roles.Select(x => new RoleListItem(
            x.Id, x.Name, userCounts.GetValueOrDefault(x.Id), permissions.GetValueOrDefault(x.Id, []))).ToList();
        return Result<IReadOnlyList<RoleListItem>>.Success(items);
    }
}
