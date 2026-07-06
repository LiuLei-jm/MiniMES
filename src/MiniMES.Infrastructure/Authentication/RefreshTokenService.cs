using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Infrastructure.Authentication;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IApplicationDbContext _context;

    public RefreshTokenService(IApplicationDbContext context)
    {
        _context = context;
    }
    public RefreshToken Create(Guid userId)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    }
    public async Task<RefreshToken?> GetAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);
    }
    public void Revoke(RefreshToken token, string? replacedBy = null)
    {
        token.RevokedAt = DateTime.UtcNow;
        token.ReplacedByToken = replacedBy;
    }

    public Task RotateAsync(RefreshToken oldToken, RefreshToken newToken)
    {
        oldToken.Revoke(newToken.Token);
        return Task.CompletedTask;
    }
}
