using MiniMES.Application.Common.Logging;

namespace MiniMES.Application.Common.Interfaces;

public interface ISecurityAuditService
{
    Task LogAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
}
