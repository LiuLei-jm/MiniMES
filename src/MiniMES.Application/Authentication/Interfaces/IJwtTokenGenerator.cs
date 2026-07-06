using MiniMES.Application.Authentication.Models;

namespace MiniMES.Application.Authentication.Interfaces;

public interface IJwtTokenGenerator
{
    JwtTokenResult Generate(JwtUserInfo userInfo);
}
