using Microsoft.AspNetCore.Http;
using MiniMES.Application.Common.Security;

namespace MiniMES.Infrastructure.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _contextAccessor;
    public CurrentUser(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    public Guid UserId => Guid.Parse(_contextAccessor.HttpContext?.User?.FindFirst("userId")?.Value ?? Guid.Empty.ToString());

    public string UserName => _contextAccessor.HttpContext?.User?.Identity?.Name ?? "";

    public List<string> Roles => _contextAccessor.HttpContext?.User?
        .Claims.Where(c => c.Type == ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList() ?? [];

    public bool IsAuthenticated => _contextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
