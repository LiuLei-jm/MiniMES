using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Logging;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Persistence;

namespace MiniMES.Infrastructure.Authentication;

public sealed class SecurityAuditService(AppDbContext db, ICurrentRequestContext requestContext) : ISecurityAuditService
{
    public async Task LogAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog(auditEvent.Category, auditEvent.Action, auditEvent.Success, DateTime.UtcNow)
        {
            UserId = Guid.TryParse(requestContext.UserId, out var userId) ? userId : null,
            UserName = auditEvent.UserName ?? requestContext.UserName,
            TargetType = auditEvent.TargetType,
            TargetId = auditEvent.TargetId,
            IpAddress = requestContext.ClientIp,
            TraceId = requestContext.TraceId,
            Details = auditEvent.Details
        };
        db.AuditLogs.Add(log);
        await db.SaveChangesAsync(cancellationToken);
    }
}
