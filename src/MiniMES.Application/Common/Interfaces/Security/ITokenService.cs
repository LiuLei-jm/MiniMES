using MiniMES.Application.Common.Security;
using MiniMES.Domain.Entities;

namespace MiniMES.Application.Common.Interfaces.Security;

public interface ITokenService
{
    Task<TokenResult> GenerateAccessPairAsync(User user);
}
