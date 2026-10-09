using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Users.Models;

namespace MiniMES.Application.Features.Users.GetUsers;

public sealed class GetUsersQueryHandler(IApplicationDbContext db) : IRequestHandler<GetUsersQuery, Result<UserPage>>
{
    public async Task<Result<UserPage>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = db.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x => x.UserName.Contains(keyword));
        }

        var total = await query.CountAsync(cancellationToken);
        var users = await query.OrderBy(x => x.UserName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new { x.Id, x.UserName, x.IsActive, x.CreatedTime, x.LastLoginAt })
            .ToListAsync(cancellationToken);
        var userIds = users.Select(x => x.Id).ToList();
        var assignments = await (from userRole in db.UserRoles
                                 join role in db.Roles on userRole.RoleId equals role.Id
                                 where userIds.Contains(userRole.UserId)
                                 select new { userRole.UserId, role.Name })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var roles = assignments.GroupBy(x => x.UserId)
            .ToDictionary(x => x.Key, x => (IReadOnlyList<string>)x.Select(y => y.Name).Order().ToList());
        var items = users.Select(x => new UserListItem(
            x.Id, x.UserName, x.IsActive, x.CreatedTime, x.LastLoginAt,
            roles.GetValueOrDefault(x.Id, []))).ToList();

        return Result<UserPage>.Success(new UserPage(items, total, request.Page, request.PageSize));
    }
}
