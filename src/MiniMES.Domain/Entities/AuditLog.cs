using MiniMES.Domain.Common;

namespace MiniMES.Domain.Entities;

public sealed class AuditLog : BaseEntity
{
    private AuditLog() { }

    public AuditLog(string category, string action, bool success, DateTime occurredAt)
    {
        Id = Guid.NewGuid();
        Category = category;
        Action = action;
        Success = success;
        OccurredAt = occurredAt;
    }

    public string Category { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public bool Success { get; private set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? TargetType { get; set; }
    public string? TargetId { get; set; }
    public string? IpAddress { get; set; }
    public string? TraceId { get; set; }
    public string? Details { get; set; }
    public DateTime OccurredAt { get; private set; }
}
