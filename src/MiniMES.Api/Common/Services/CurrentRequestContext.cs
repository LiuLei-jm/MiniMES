using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Api.Common.Services;

public sealed class CurrentRequestContext(IHttpContextAccessor accessor) : ICurrentRequestContext
{
    public string TraceId => accessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");
    public string? UserId => accessor.HttpContext?.User?.FindFirst("userId")?.Value;
    public string? UserName => accessor.HttpContext?.User?.Identity?.Name;
    public string ClientIp => accessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
