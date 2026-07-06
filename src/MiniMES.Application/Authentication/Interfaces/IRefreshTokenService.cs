using MiniMES.Domain.Entities;

namespace MiniMES.Application.Authentication.Interfaces;

public interface IRefreshTokenService
{
    RefreshToken Create(Guid userId);
    Task<RefreshToken?> GetAsync(string token);
    Task RotateAsync(RefreshToken oldToken, RefreshToken newToken);
    void Revoke(RefreshToken refreshToken, string? replacedBy = null);
}
