using MiniMES.Domain.Entities;

namespace MiniMES.Application.Common.Security;

public interface IJwtTokenGenerator
{
    Task<string> GenerateAsync(User user, List<string> roles, List<string> permissions);
}
