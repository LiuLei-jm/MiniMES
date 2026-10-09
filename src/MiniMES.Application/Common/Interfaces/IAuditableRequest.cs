namespace MiniMES.Application.Common.Interfaces;

public interface IAuditableRequest
{
    string AuditAction { get; }
    string AuditTargetType { get; }
    string? AuditTargetId { get; }
}
