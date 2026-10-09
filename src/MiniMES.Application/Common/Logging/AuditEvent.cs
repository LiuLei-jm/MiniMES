namespace MiniMES.Application.Common.Logging;

public sealed record AuditEvent(
    string Category,
    string Action,
    bool Success,
    string? UserName = null,
    string? TargetType = null,
    string? TargetId = null,
    string? Details = null);
