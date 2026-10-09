using MiniMES.Domain.Entities;

namespace MiniMES.Application.Authentication.Models;

public sealed class IssuedRefreshToken
{
    public required string Token { get; init; }
    public required RefreshToken Entity { get; init; }
}
