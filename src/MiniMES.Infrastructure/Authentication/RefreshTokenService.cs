using Microsoft.Extensions.Options;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;
using MiniMES.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace MiniMES.Infrastructure.Authentication;

public sealed class RefreshTokenService(IOptions<JwtOptions> options) : IRefreshTokenService
{
    private readonly JwtOptions _options = options.Value;

    public IssuedRefreshToken Create(Guid userId, string? createdByIp = null)
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        return new IssuedRefreshToken
        {
            Token = token,
            Entity = new RefreshToken
            {
                Id = Guid.NewGuid(), UserId = userId, TokenHash = Hash(token), CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpireDays), CreatedByIp = createdByIp
            }
        };
    }

    public string Hash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
