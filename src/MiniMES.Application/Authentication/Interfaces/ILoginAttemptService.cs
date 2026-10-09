namespace MiniMES.Application.Authentication.Interfaces;

public interface ILoginAttemptService
{
    Task<bool> IsBlockedAsync(string username, string clientIp, CancellationToken cancellationToken = default);
    Task RecordFailureAsync(string username, string clientIp, CancellationToken cancellationToken = default);
    Task ResetAsync(string username, string clientIp, CancellationToken cancellationToken = default);
}
