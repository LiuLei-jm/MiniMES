namespace MiniMES.Application.Common.Interfaces;

public interface ICurrentRequestContext
{
    string TraceId { get; }
    string? UserId { get; }
}
