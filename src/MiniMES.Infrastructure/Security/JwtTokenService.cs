using MiniMES.Application.Common.Interfaces.Security;
using MiniMES.Application.Common.Security;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Identity;

namespace MiniMES.Infrastructure.Security;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public Task<TokenResult> GenerateAccessPairAsync(User user)
    {
        throw new NotImplementedException();
    }
}
