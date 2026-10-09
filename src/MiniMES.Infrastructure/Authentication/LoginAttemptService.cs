using Microsoft.Extensions.Caching.Distributed;
using MiniMES.Application.Authentication.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MiniMES.Infrastructure.Authentication;

public sealed class LoginAttemptService(IDistributedCache cache) : ILoginAttemptService
{
    private const int MaximumAttempts = 10;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public async Task<bool> IsBlockedAsync(string username, string clientIp, CancellationToken cancellationToken = default) =>
        int.TryParse(await cache.GetStringAsync(Key(username, clientIp), cancellationToken), out var count) && count >= MaximumAttempts;

    public async Task RecordFailureAsync(string username, string clientIp, CancellationToken cancellationToken = default)
    {
        var key = Key(username, clientIp);
        var count = int.TryParse(await cache.GetStringAsync(key, cancellationToken), out var existing) ? existing + 1 : 1;
        await cache.SetStringAsync(key, count.ToString(), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Window }, cancellationToken);
    }

    public Task ResetAsync(string username, string clientIp, CancellationToken cancellationToken = default) => cache.RemoveAsync(Key(username, clientIp), cancellationToken);

    private static string Key(string username, string clientIp) => $"login-attempt:{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{username.Trim().ToUpperInvariant()}:{clientIp}")))}";
}
