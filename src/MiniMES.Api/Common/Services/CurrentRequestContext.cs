using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Api.Common.Services;

public class CurrentRequestContext : ICurrentRequestContext
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentRequestContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string TraceId => _accessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString("N");

    public string? UserId => _accessor.HttpContext?.User?.Identity?.Name;
}
