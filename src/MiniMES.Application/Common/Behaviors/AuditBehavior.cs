using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Logging;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Behaviors;

public sealed class AuditBehavior<TRequest, TResponse>(ISecurityAuditService auditService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not IAuditableRequest auditable) return await next();

        try
        {
            var response = await next();
            var success = response is not Result result || result.IsSuccess;
            await auditService.LogAsync(new AuditEvent(
                "Operation", auditable.AuditAction, success,
                TargetType: auditable.AuditTargetType,
                TargetId: auditable.AuditTargetId,
                Details: success ? null : (response as Result)?.Error.Code), cancellationToken);
            return response;
        }
        catch (Exception exception)
        {
            await auditService.LogAsync(new AuditEvent(
                "Operation", auditable.AuditAction, false,
                TargetType: auditable.AuditTargetType,
                TargetId: auditable.AuditTargetId,
                Details: exception.GetType().Name), cancellationToken);
            throw;
        }
    }
}
