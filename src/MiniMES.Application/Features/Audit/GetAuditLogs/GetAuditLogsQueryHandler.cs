using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Audit.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetAuditLogsQuery, Result<AuditLogPage>>
{
    public async Task<Result<AuditLogPage>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var query = db.AuditLogs.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Category)) query = query.Where(x => x.Category == request.Category);
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(x => (x.UserName != null && x.UserName.Contains(keyword)) || x.Action.Contains(keyword) || (x.TargetId != null && x.TargetId.Contains(keyword)));
        }
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.OccurredAt)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize)
            .Select(x => new AuditLogItem(x.Id, x.Category, x.Action, x.Success, x.UserId, x.UserName, x.TargetType, x.TargetId, x.IpAddress, x.TraceId, x.Details, x.OccurredAt))
            .ToListAsync(cancellationToken);
        return Result<AuditLogPage>.Success(new AuditLogPage(items, total, request.Page, request.PageSize));
    }
}
