using MiniMES.Application.Authentication.Models;

namespace MiniMES.Application.Authentication.Interfaces;

public interface IRefreshTokenService
{
    IssuedRefreshToken Create(Guid userId, string? createdByIp = null);
    string Hash(string token);
}
