using MiniMES.Application.Authentication.Constrants;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;

namespace MiniMES.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }
    public JwtTokenResult Generate(JwtUserInfo userInfo)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.ExpireMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub,userInfo.UserId.ToString()),
            new Claim(ClaimTypes.Name, userInfo.UserName),
            new Claim(CustomClaims.UserId, userInfo.UserId.ToString())
        };
        claims.AddRange(userInfo.Roles.Select(r => new Claim(ClaimTypes.Role, r)));
        claims.AddRange(userInfo.Permissions.Select(p => new Claim(CustomClaims.Permission, p)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpireMinutes),
            signingCredentials: creds
            );
        return new JwtTokenResult
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }

}
