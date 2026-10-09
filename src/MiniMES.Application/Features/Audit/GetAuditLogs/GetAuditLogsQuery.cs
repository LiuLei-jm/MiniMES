using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Audit.GetAuditLogs;

public sealed record AuditLogItem(
    Guid Id,
    string Category,
    string Action,
    bool Success,
    Guid? UserId,
    string? UserName,
    string? TargetType,
    string? TargetId,
    string? IpAddress,
    string? TraceId,
    string? Details,
    DateTime OccurredAt);

public sealed record AuditLogPage(IReadOnlyList<AuditLogItem> Items, int Total, int Page, int PageSize);
public sealed record GetAuditLogsQuery(string? Category, string? Keyword, int Page, int PageSize) : IRequest<Result<AuditLogPage>>;

public sealed class GetAuditLogsQueryValidator : AbstractValidator<GetAuditLogsQuery>
{
    public GetAuditLogsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Category).MaximumLength(30);
        RuleFor(x => x.Keyword).MaximumLength(100);
    }
}
