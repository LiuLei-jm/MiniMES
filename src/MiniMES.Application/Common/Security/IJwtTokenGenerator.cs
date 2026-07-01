using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Security;

public interface IJwtTokenGenerator
{
    string Generate(JwtUserInfo userInfo);
    DateTime GetExpireTime();
}
