using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Audit.GetAuditLogs;

namespace MiniMES.Api.Endpoints.Audit;

public sealed class GetAuditLogsRequest
{
    public string? Category { get; init; }
    public string? Keyword { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

[Permission("audit:view")]
public sealed class GetAuditLogsEndpoint(ISender sender) : Endpoint<GetAuditLogsRequest, AuditLogPage>
{
    public override void Configure() { Get("/api/audit-logs"); Policies("permission:audit:view"); Summary(x => x.Summary = "分页查询安全、操作与登录日志"); }
    public override async Task HandleAsync(GetAuditLogsRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new GetAuditLogsQuery(request.Category, request.Keyword, request.Page, request.PageSize), ct);
        await Send.OkAsync(result.GetValue(), ct);
    }
}
